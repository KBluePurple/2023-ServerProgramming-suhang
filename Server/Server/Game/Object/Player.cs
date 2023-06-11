using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class Player : GameObject
    {
        public Player()
        {
            ObjectType = GameObjectType.Player;
        }

        public ClientSession Session { get; set; }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }
    }
}