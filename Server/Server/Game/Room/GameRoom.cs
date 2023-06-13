using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game
{
    public class GameRoom : JobSerializer
    {
        private readonly Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();

        private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();
        private readonly Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
        public int RoomId { get; set; }

        public Map Map { get; } = new Map();

        public void Init(int mapId)
        {
            Map.LoadMap(mapId);

            // TEMP
            var monster = ObjectManager.Instance.Add<Monster>();
            monster.CellPos = new Vector2Int(5, 5);
            EnterGame(monster);
        }

        // 누군가 주기적으로 호출해줘야 한다
        public void Update()
        {
            foreach (var monster in _monsters.Values) monster.Update();

            foreach (var projectile in _projectiles.Values) projectile.Update();

            Flush();
        }

        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            var type = ObjectManager.GetObjectTypeById(gameObject.Id);

            if (type == GameObjectType.Player)
            {
                var player = gameObject as Player;
                _players.Add(gameObject.Id, player);
                player.Room = this;

                Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y));

                // 본인한테 정보 전송
                {
                    var enterPacket = new S_EnterGame();
                    enterPacket.Player = player.Info;
                    player.Session.Send(enterPacket);

                    var spawnPacket = new S_Spawn();
                    foreach (var p in _players.Values)
                        if (player != p)
                            spawnPacket.Objects.Add(p.Info);

                    foreach (var m in _monsters.Values)
                        spawnPacket.Objects.Add(m.Info);

                    foreach (var p in _projectiles.Values)
                        spawnPacket.Objects.Add(p.Info);

                    player.Session.Send(spawnPacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                var monster = gameObject as Monster;
                _monsters.Add(gameObject.Id, monster);
                monster.Room = this;

                Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));
            }
            else if (type == GameObjectType.Projectile)
            {
                var projectile = gameObject as Projectile;
                _projectiles.Add(gameObject.Id, projectile);
                projectile.Room = this;
            }

            // 타인한테 정보 전송
            {
                var spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                foreach (var p in _players.Values)
                    if (p.Id != gameObject.Id)
                        p.Session.Send(spawnPacket);
            }
        }

        public void LeaveGame(int objectId)
        {
            var type = ObjectManager.GetObjectTypeById(objectId);

            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(objectId, out player) == false)
                    return;

                Map.ApplyLeave(player);
                player.Room = null;

                // 본인한테 정보 전송
                {
                    var leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = null;
                if (_monsters.Remove(objectId, out monster) == false)
                    return;

                Map.ApplyLeave(monster);
                monster.Room = null;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile) == false)
                    return;

                projectile.Room = null;
            }

            // 타인한테 정보 전송
            {
                var despawnPacket = new S_Despawn();
                despawnPacket.ObjectIds.Add(objectId);
                foreach (var p in _players.Values)
                    if (p.Id != objectId)
                        p.Session.Send(despawnPacket);
            }
        }

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;

            // TODO : 검증
            var movePosInfo = movePacket.PosInfo;
            var info = player.Info;

            // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
            if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
                if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                    return;

            info.PosInfo.State = movePosInfo.State;
            info.PosInfo.MoveDir = movePosInfo.MoveDir;
            Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

            // 다른 플레이어한테도 알려준다
            var resMovePacket = new S_Move();
            resMovePacket.ObjectId = player.Info.ObjectId;
            resMovePacket.PosInfo = movePacket.PosInfo;

            Broadcast(resMovePacket);
        }

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;

            var info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle)
                return;

            // TODO : 스킬 사용 가능 여부 체크
            info.PosInfo.State = CreatureState.Skill;
            var skill = new S_Skill
            {
                Info = new SkillInfo(),
                ObjectId = info.ObjectId
            };
            skill.Info.SkillId = skillPacket.Info.SkillId;
            Broadcast(skill);

            if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out var skillData) == false)
                return;

            switch (skillData.skillType)
            {
                case SkillType.SkillAuto:
                {
                    var skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                    var target = Map.Find(skillPos);
                    if (target != null) Console.WriteLine("Hit GameObject !");
                }
                    break;
                case SkillType.SkillProjectile:
                {
                    var arrow = ObjectManager.Instance.Add<Arrow>();
                    if (arrow == null)
                        return;

                    arrow.Owner = player;
                    arrow.Data = skillData;
                    arrow.PosInfo.State = CreatureState.Moving;
                    arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                    arrow.PosInfo.PosX = player.PosInfo.PosX;
                    arrow.PosInfo.PosY = player.PosInfo.PosY;
                    arrow.Speed = skillData.projectile.speed;
                    Push(EnterGame, arrow);
                }
                    break;
                case SkillType.SkillHeal:
                    player.Hp += 10;
                    
                    var healthChangePacket = new S_ChangeHp
                    {
                        ObjectId = player.Info.ObjectId,
                        Hp = player.Hp
                    };
                    Broadcast(healthChangePacket);
                    Console.WriteLine("Heal");
                    break;
                case SkillType.SkillNone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Player FindPlayer(Func<GameObject, bool> condition)
        {
            return _players.Values.FirstOrDefault(condition.Invoke);
        }

        public void Broadcast(IMessage packet)
        {
            foreach (var p in _players.Values) p.Session.Send(packet);
        }

        public void HandleChat(Player player, C_Msg chatPacket)
        {
            if (player == null)
                return;

            var chat = new S_Msg
            {
                PlayerId = player.Info.ObjectId,
                Message = chatPacket.Message
            };

            Broadcast(chat);
        }
    }
}