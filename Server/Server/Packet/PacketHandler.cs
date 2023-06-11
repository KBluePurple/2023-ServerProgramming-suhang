using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;

internal class PacketHandler
{
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        var movePacket = packet as C_Move;
        var clientSession = session as ClientSession;

        //Console.WriteLine($"C_Move ({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");

        var player = clientSession.MyPlayer;
        if (player == null)
            return;

        var room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleMove, player, movePacket);
    }

    public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
        var skillPacket = packet as C_Skill;
        var clientSession = session as ClientSession;

        var player = clientSession.MyPlayer;
        if (player == null)
            return;

        var room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleSkill, player, skillPacket);
    }
    
    public static void C_MsgHandler(PacketSession session, IMessage packet)
    {
        var chatPacket = packet as C_Msg;
        var clientSession = session as ClientSession;

        var player = clientSession.MyPlayer;
        if (player == null)
            return;

        var room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleChat, player, chatPacket);
    }
}