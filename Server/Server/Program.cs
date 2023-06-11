using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Server.Data;
using Server.Game;
using ServerCore;
using Timer = System.Timers.Timer;

namespace Server
{
    internal class Program
    {
        private static readonly Listener _listener = new Listener();
        private static readonly List<Timer> _timers = new List<Timer>();

        private static void TickRoom(GameRoom room, int tick = 100)
        {
            var timer = new Timer();
            timer.Interval = tick;
            timer.Elapsed += (s, e) => { room.Update(); };
            timer.AutoReset = true;
            timer.Enabled = true;

            _timers.Add(timer);
        }

        private static void Main(string[] args)
        {
            ConfigManager.LoadConfig();
            DataManager.LoadData();

            var room = RoomManager.Instance.Add(1);
            TickRoom(room, 50);

            // DNS (Domain Name System)
            // var host = Dns.GetHostName();
            // var ipHost = Dns.GetHostEntry(host);
            // var ipAddr = ipHost.AddressList[0];
            var endPoint = new IPEndPoint(IPAddress.Any, 7777);

            _listener.Init(endPoint, () => SessionManager.Instance.Generate());
            Console.WriteLine("Listening...");

            //FlushRoom();
            //JobTimer.Instance.Push(FlushRoom);

            // TODO
            while (true)
                //JobTimer.Instance.Flush();
                Thread.Sleep(100);
        }
    }
}