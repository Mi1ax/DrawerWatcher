using Riptide;
using Riptide.Transports.Udp;
using Riptide.Utils;

namespace CouscousEngine.Networking;

public static class ClientManager
{
    public static Client? Client { get; set; }
    private static RiptideLogger.LogMethod? LogMethod;
    
    public static void SetLogger(RiptideLogger.LogMethod logMethod)
        => LogMethod = logMethod;

    public static void Initialize(
        EventHandler onConnected,
        EventHandler<ClientDisconnectedEventArgs> onClientDisconnected) 
    {
        RiptideLogger.Initialize(LogMethod ??= Console.WriteLine, true);
        Client = new Client();
        Client.ChangeTransport(new UdpClient(SocketMode.IPv4Only));
        Client.Connected += onConnected;
        Client.ClientDisconnected += onClientDisconnected;
    }

    public static bool Connect(string ip, ushort port)
    {
        if (Client != null) 
            return Client.Connect($"{ip}:{port}");
        
        Console.WriteLine("[ERROR] Client instance is null!");
        return false;
    }

    public static void Update()
    {
        if (Client == null)
        {
            Console.WriteLine("[ERROR] Client instance is null!");
            return;
        }

        Client.Update();
    }
}