using System.Net;
using Google.Protobuf;
using ServerCore;

public class NetworkManager
{
    private readonly ServerSession _session = new();

    public void Send(IMessage packet)
    {
        _session.Send(packet);
    }

    public void Init()
    {
        // DNS (Domain Name System)
        // var host = Dns.GetHostName();
        // var ipHost = Dns.GetHostEntry(host);
        // var ipAddr = ipHost.AddressList[0];
        var endPoint = new IPEndPoint(IPAddress.Parse("167.179.89.42"), 7777);

        var connector = new Connector();

        connector.Connect(endPoint, () => _session);
    }

    public void Update()
    {
        var list = PacketQueue.Instance.PopAll();
        foreach (var packet in list)
        {
            var handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            handler?.Invoke(_session, packet.Message);
        }
    }
}