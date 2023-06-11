using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers s_instance; // 유일성이 보장된다

    private static Managers Instance
    {
        get
        {
            Init();
            return s_instance;
        }
    } // 유일한 매니저를 갖고온다

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        _network.Update();
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            var go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._network.Init();
            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._sound.Init();
        }
    }

    public static void Clear()
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }

    #region Contents

    private readonly MapManager _map = new();
    private readonly ObjectManager _obj = new();
    private readonly NetworkManager _network = new();

    public static MapManager Map => Instance._map;
    public static ObjectManager Object => Instance._obj;
    public static NetworkManager Network => Instance._network;

    #endregion

    #region Core

    private readonly DataManager _data = new();
    private readonly PoolManager _pool = new();
    private readonly ResourceManager _resource = new();
    private readonly SceneManagerEx _scene = new();
    private readonly SoundManager _sound = new();
    private readonly UIManager _ui = new();

    public static DataManager Data => Instance._data;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => Instance._ui;

    #endregion
}