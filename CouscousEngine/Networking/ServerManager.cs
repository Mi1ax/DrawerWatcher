using Riptide;
using Riptide.Experimental.TcpTransport;
using Riptide.Utils;

namespace CouscousEngine.Networking;

public static class ServerManager
{
    private static Server? _server;

    public static Server Server
    {
        get
        {
            if (_server == null)
                throw new NullReferenceException("[ERROR] Server instance is null!");
            return _server;
        }
    }

    public static void Initialize(
        EventHandler<ServerConnectedEventArgs> onClientConnected, 
        EventHandler<ServerDisconnectedEventArgs> onClientDisconnected
        )
    {
        RiptideLogger.Initialize(Console.WriteLine, true);

        _server = new Server();

        Server.ClientConnected += onClientConnected;
        Server.ClientDisconnected += onClientDisconnected;
    }

    public static void Start(ushort port, ushort maxConnection)
    {
        Server.Start(port, maxConnection);
    }

    public static void Stop()
    {
        Server.Stop();
    }
    
    public static void Update()
    {
        Server.Update();
    }
}