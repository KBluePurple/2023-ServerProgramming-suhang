using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class ObjectManager
    {
        // [UNUSED(1)][TYPE(7)][ID(24)]
        private int _counter;

        private readonly object _lock = new object();
        private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();
        public static ObjectManager Instance { get; } = new ObjectManager();

        public T Add<T>() where T : GameObject, new()
        {
            var gameObject = new T();

            lock (_lock)
            {
                gameObject.Id = GenerateId(gameObject.ObjectType);

                if (gameObject.ObjectType == GameObjectType.Player) _players.Add(gameObject.Id, gameObject as Player);
            }

            return gameObject;
        }

        private int GenerateId(GameObjectType type)
        {
            lock (_lock)
            {
                return ((int)type << 24) | _counter++;
            }
        }

        public static GameObjectType GetObjectTypeById(int id)
        {
            var type = (id >> 24) & 0x7F;
            return (GameObjectType)type;
        }

        public bool Remove(int objectId)
        {
            var objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                    return _players.Remove(objectId);
            }

            return false;
        }

        public Player Find(int objectId)
        {
            var objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    Player player = null;
                    if (_players.TryGetValue(objectId, out player))
                        return player;
                }
            }

            return null;
        }
    }
}