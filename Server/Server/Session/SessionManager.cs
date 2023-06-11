using System;
using System.Collections.Generic;

namespace Server
{
    internal class SessionManager
    {
        private readonly object _lock = new object();

        private int _sessionId;
        private readonly Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        public static SessionManager Instance { get; } = new SessionManager();

        public ClientSession Generate()
        {
            lock (_lock)
            {
                var sessionId = ++_sessionId;

                var session = new ClientSession();
                session.SessionId = sessionId;
                _sessions.Add(sessionId, session);

                Console.WriteLine($"Connected : {sessionId}");

                return session;
            }
        }

        public ClientSession Find(int id)
        {
            lock (_lock)
            {
                ClientSession session = null;
                _sessions.TryGetValue(id, out session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionId);
            }
        }
    }
}