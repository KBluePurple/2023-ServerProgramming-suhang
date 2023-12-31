using System;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

internal class PacketManager
{
    private readonly Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new();

    private readonly Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new();

    private PacketManager()
    {
        Register();
    }

    public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

    public void Register()
    {
        _onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
        _handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);
        _onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
        _handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);
        _onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
        _handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);
        _onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
        _handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);
        _onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
        _handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);
        _onRecv.Add((ushort)MsgId.SSkill, MakePacket<S_Skill>);
        _handler.Add((ushort)MsgId.SSkill, PacketHandler.S_SkillHandler);
        _onRecv.Add((ushort)MsgId.SChangeHp, MakePacket<S_ChangeHp>);
        _handler.Add((ushort)MsgId.SChangeHp, PacketHandler.S_ChangeHpHandler);
        _onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
        _handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);
        _onRecv.Add((ushort)MsgId.SMsg, MakePacket<S_Msg>);
        _handler.Add((ushort)MsgId.SMsg, PacketHandler.S_MsgHandler);
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        var size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        var id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>, ushort> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer, id);
    }

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        var pkt = new T();
        pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

        if (CustomHandler != null)
        {
            CustomHandler.Invoke(session, pkt, id);
        }
        else
        {
            Action<PacketSession, IMessage> action = null;
            if (_handler.TryGetValue(id, out action))
                action.Invoke(session, pkt);
        }
    }

    public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
    {
        Action<PacketSession, IMessage> action = null;
        if (_handler.TryGetValue(id, out action))
            return action;
        return null;
    }

    #region Singleton

    public static PacketManager Instance { get; } = new();

    #endregion
}