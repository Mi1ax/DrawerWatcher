using CouscousEngine.Networking;

namespace Drawer_Watcher.Managers;

public static class NetworkManager
{
    public static bool IsConnectedToServer => _isHost || ClientManager.Client!.IsConnected;

    #region Server
    
    private static bool _isHost;

    public static bool IsHost
    {
        get => _isHost;
        set
        {
            _isHost = value;
            if (_isHost) InitializeServer();
            else CloseServer();
        }
    }

    private static void InitializeServer()
    {
        ServerManager.Initialize(
            (_, e) =>
            {
                new Player(e.Client.Id);
            },
            (_, e) =>
            {
                GameManager.Players.Remove(e.Client.Id);
            }
        );
        ServerManager.Start((ushort)ConnectionInfo.Port, ConnectionInfo.MaxConnection);
    }

    private static void CloseServer()
    {
        ServerManager.Stop();
    }

    public static bool ServerIsRunning() => ServerManager.Server.IsRunning;
    
    #endregion
    
    #region Client

    private static bool _isConnect;
    
    public static void Initialize()
    {
        ClientManager.Initialize((_, e) =>
        {
            GameManager.Players.Remove(e.Id);
        });
    }

    public static bool Connect()
    {
        _isConnect = ClientManager.Connect(ConnectionInfo.Ip, (ushort)ConnectionInfo.Port);
        return _isConnect;
    }
    
    #endregion
    
    public static void Update()
    {
        if (IsHost) ServerManager.Update();
        ClientManager.Update();
    }
}