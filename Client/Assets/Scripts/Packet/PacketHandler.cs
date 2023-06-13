using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

public static class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        if (packet is S_EnterGame enterGamePacket) Managers.Object.Add(enterGamePacket.Player, true);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        Managers.Object.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Spawn spawnPacket) return;
        foreach (var obj in spawnPacket.Objects)
            Managers.Object.Add(obj);
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Despawn despawnPacket) return;
        foreach (var id in despawnPacket.ObjectIds)
            Managers.Object.Remove(id);
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Move movePacket) return;
        var go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;

        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
            return;

        var bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;

        bc.PosInfo = movePacket.PosInfo;
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Skill skillPacket) return;
        var go = Managers.Object.FindById(skillPacket.ObjectId);
        if (go == null)
            return;

        var cc = go.GetComponent<CreatureController>();
        if (cc != null) cc.UseSkill(skillPacket.Info.SkillId);
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ChangeHp changePacket) return;
        
        var go = Managers.Object.FindById(changePacket.ObjectId);
        if (go == null)
            return;

        var cc = go.GetComponent<CreatureController>();
        if (cc != null) cc.Hp = changePacket.Hp;
    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Die diePacket) return;
        
        var go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null)
            return;

        var cc = go.GetComponent<CreatureController>();
        
        if (cc == null) return;
        
        cc.Hp = 0;
        cc.OnDead();
    }

    public static void S_MsgHandler(PacketSession session, IMessage packet)
    {
        if (packet is S_Msg msgPacket)
            Managers.UI.Chat.AddChat($"{msgPacket.PlayerId}: {msgPacket.Message}");
    }
}