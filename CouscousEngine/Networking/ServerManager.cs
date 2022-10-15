﻿using Riptide;
using Riptide.Experimental.TcpTransport;
using Riptide.Transports.Udp;
using Riptide.Utils;
using System.Data;

namespace CouscousEngine.Networking;

public static class ServerManager
{
    private static Server? _server;
    private static RiptideLogger.LogMethod? LogMethod = null;

    public static Server Server
    {
        get
        {
            if (_server == null)
                throw new NullReferenceException("[ERROR] Server instance is null!");
            return _server;
        }
    }

    public static void SetLogger(RiptideLogger.LogMethod logMethod)
        => LogMethod = logMethod;
    
    public static void Initialize(
        EventHandler<ServerConnectedEventArgs> onClientConnected, 
        EventHandler<ServerDisconnectedEventArgs> onClientDisconnected
        )
    {
        RiptideLogger.Initialize(LogMethod ??= Console.WriteLine, true);

        _server = new Server();
        _server.ChangeTransport(new UdpServer(SocketMode.IPv4Only));

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