using CouscousEngine.Networking;
using Drawer_Watcher.Screens;
using Riptide;
using Riptide.Utils;

namespace Drawer_Watcher.Managers;

public enum MessageID : ushort
{
    SendPainting = 1,
    SendPosition,
    DrawerChanged,
    AllClear,
    
    ChatMessage,
    
    StartGame,
    SendWord,
    SendWinning
}

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

    public static void StartGame()
    {
        // Send from Client to Server
        var message = Message.Create(MessageSendMode.Reliable, MessageID.StartGame);
        message.AddString(GameLogic.CurrentWord);
        ClientManager.Client!.Send(message);
    }
    
    [MessageHandler((ushort) MessageID.StartGame)]
    private static void ReceiveStartGameHandler(Message message)
    {
        // Get data from Server to Client
        GameLogic.CurrentWord = message.GetString();
        RiptideLogger.Log(
            LogType.Info, 
            $"Recieve start from server to client: {GameLogic.CurrentWord}"
            );
        ScreenManager.NavigateTo(new GameScreen());
    }
    
    [MessageHandler((ushort) MessageID.SendWinning)]
    private static void ReceiveWinningHandler(Message message)
    {
        // Get data from Server to Client
        RiptideLogger.Log(LogType.Info, "Get from server to client");
        var guesser = message.GetUShort();
        GameLogic.Winner = guesser;
    }
    
    [MessageHandler((ushort) MessageID.SendWinning)]
    private static void ReceiveWinningHandler(ushort fromClientID, Message message)
    {
        RiptideLogger.Log(LogType.Info, "Get from client to server");
        // Receive data from Client and use in Server 
        ServerManager.Server.SendToAll(message);
    }
    
    [MessageHandler((ushort) MessageID.StartGame)]
    private static void ReceiveStartGameHandler(ushort fromClientID, Message message)
    {
        // Receive data from Client and use in Server 
        RiptideLogger.Log(LogType.Info, nameof(ReceiveStartGameHandler));
        ServerManager.Server.SendToAll(message);
    }
    
    public static void Update()
    {
        if (IsHost) ServerManager.Update();
        ClientManager.Update();
    }
}