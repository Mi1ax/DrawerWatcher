using System.Numerics;
using System.Reflection;
using CouscousEngine.Core;
using CouscousEngine.Networking;
using CouscousEngine.Utils;
using Drawer_Watcher.Panels;
using Drawer_Watcher.Screens;
using ImGuiNET;
using Riptide;
using Riptide.Utils;

namespace Drawer_Watcher.Managers;

public enum MessageID : ushort
{
    SendPainting = 1,
    ChatMessage,
    
    StartGame,
    SendWinning,
    
    ConnectionInfo,
    DrawerStatus,
    AllClear,
}

public static class NetworkLogger
{
    public static readonly List<string> LoggerMessages = new();

    public static void Init()
    {
        ServerManager.SetLogger(Log);
        ClientManager.SetLogger(Log);
    }

    public static void Log(string fmt)
    {
        LoggerMessages.Add(fmt);
    }

    public static void UpdateImGuiConsole()
    {
        ImGui.Begin("Network Console");
        foreach (var loggerMessage in LoggerMessages)
            ImGui.Text(loggerMessage);
        
        if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
            ImGui.SetScrollHereY(1.0f);
        ImGui.End();
    }
}

// ReSharper disable once UnusedType.Local
public static class MessageHandlers 
{
    private static void Log(string functionName, string enumName, LogType type = LogType.Info)
    {
        RiptideLogger.Log(type, nameof(MessageHandlers), $"{enumName}: {functionName}");
    }

    private static void Log(string message, LogType type = LogType.Info)
    {
        RiptideLogger.Log(type, message);
    }
    
    #region Receive data from Server and handle on Client side

    [MessageHandler((ushort) MessageID.ChatMessage)]
    private static void ReceiveChatMessage(Message message)
    {
        Log(nameof(ReceiveChatMessage), nameof(MessageID.ChatMessage));
        var senderID = message.GetUShort();
        var text = message.GetString();
        ChatPanel.AddMessage(senderID, text);
    }
    
    [MessageHandler((ushort) MessageID.StartGame)]
    private static void ReceiveStartGame(Message message)
    {
        GameLogic.CurrentWord = message.GetString();
        Log(nameof(ReceiveStartGame), nameof(MessageID.StartGame));
        ScreenManager.NavigateTo(new GameScreen());
    }
    
    [MessageHandler((ushort) MessageID.SendWinning)]
    private static void ReceiveWinning(Message message)
    {
        Log(nameof(ReceiveWinning), nameof(MessageID.SendWinning));
        var guesser = message.GetUShort();
        GameLogic.Winner = guesser;
    }

    #endregion

    #region Receive data from Client and use in Server

    [MessageHandler((ushort) MessageID.StartGame)]
    private static void ReceiveStartGame(ushort fromClientID, Message message)
    {
        Log(nameof(ReceiveStartGame), nameof(MessageID.StartGame));
        ServerManager.Server.SendToAll(message);
    }

    [MessageHandler((ushort) MessageID.ChatMessage)]
    private static void ReceiveChatMessage(ushort fromClientID, Message message)
    {
        Log(nameof(ReceiveChatMessage), nameof(MessageID.ChatMessage));
        ServerManager.Server.SendToAll(message);
    }
    
    [MessageHandler((ushort) MessageID.SendWinning)]
    private static void ReceiveWinning(ushort fromClientID, Message message)
    {
        Log(nameof(ReceiveWinning), nameof(MessageID.SendWinning));
        ServerManager.Server.SendToAll(message);
    }

    #endregion

    #region Send data from Client to Server

    public static void SendMessageInChat(ushort senderID, string text)
    {
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.ChatMessage);
        message.AddUShort(senderID);
        message.AddString(text);
        RiptideLogger.Log(LogType.Info, GameLogic.CurrentWord);
        if (text == GameLogic.CurrentWord)
        {
            var winningMessage = Message.Create(MessageSendMode.Reliable, MessageID.SendWinning);
            winningMessage.AddUShort(senderID);
            ClientManager.Client!.Send(winningMessage);
        }
        ClientManager.Client!.Send(message);
    }

    #endregion

    /*******************************************************/
    
    #region Send data from Client to the Server

    public static void SetDrawer(ushort clientID, bool value)
    {
        Log("(To Server) New drawer value");
        NetworkManager.Players[clientID].SetDrawer(value);
        var message = Message.Create(MessageSendMode.Reliable, MessageID.DrawerStatus);
        message.AddUShort(clientID);
        message.AddBool(value);
        ClientManager.Client!.Send(message);
    }
    
    public static void ClearPainting()
    {
        Log("(To Server) Wanna clear painting");
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.AllClear);
        ClientManager.Client!.Send(message);
    }
    
    public static void SendDrawingData(Vector2 start, Vector2 end, float thickness, Color color)
    {
        // Send from Client to Server
        if (ImGui.GetIO().WantCaptureMouse) return;
    
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.SendPainting);
        message.AddVector2(start);
        message.AddVector2(end);
        message.AddFloat(thickness);
        message.AddColor(color);
    
        ClientManager.Client!.Send(message);
    }

    #endregion

    #region Send data from Server to the Client/Clients

    public static void SendOtherPlayersInfo(ushort toClientId)
    {
        Log($"(To Client) Send {toClientId} other players info");
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ConnectionInfo);
        message.AddInt(NetworkManager.Players.Count);
        foreach (var (id, player) in NetworkManager.Players)
        {
            message.AddUShort(id);
            message.AddBool(player.IsDrawer);
        }
        ServerManager.Server.Send(message, toClientId);

        var newPlayer = Message.Create(MessageSendMode.Reliable, MessageID.ConnectionInfo);
        newPlayer.AddInt(1);
        newPlayer.AddUShort(toClientId);
        newPlayer.AddBool(false);
        ServerManager.Server.SendToAll(message, toClientId);
    }
    
    [MessageHandler((ushort) MessageID.DrawerStatus)]
    private static void HandleDrawerMessage(ushort fromClientID, Message message)
    {
        Log("(To all Clients) Drawer status");
        ServerManager.Server.SendToAll(message, fromClientID);
    }
    
    [MessageHandler((ushort) MessageID.AllClear)]
    private static void HandleAllClear(ushort fromClientID, Message message)
    {
        if (!NetworkManager.Players[fromClientID].IsDrawer) return;
        Log("(To all Clients) Clear painting");
        ServerManager.Server.SendToAll(message);
    }
    
    [MessageHandler((ushort) MessageID.SendPainting)]
    private static void HandlePainting(ushort fromClientID, Message message)
    {
        ServerManager.Server.SendToAll(message);
    }

    #endregion

    #region Handle Server messages on Client Side

    [MessageHandler((ushort) MessageID.ConnectionInfo)]
    private static void HandleConnectionInfo(Message message)
    {
        var playersCount = message.GetInt();
        Log($"(From Server) Get data about {playersCount} other players!");
        for (var _ = 0; _ < playersCount; _++)
        {
            var id = message.GetUShort();
            var player = new Player(id, message.GetBool());
            if (!NetworkManager.Players.ContainsKey(id))
                NetworkManager.Players.Add(id, player);
        }
    }
    
    [MessageHandler((ushort) MessageID.DrawerStatus)]
    private static void HandleDrawerStatus(Message message)
    {
        var clientID = message.GetUShort();
        var value = message.GetBool();
        var msg = value ? "is drawer" : "is not a drawer";
        Log($"(From Server) {clientID}: {msg}");
        NetworkManager.Players[clientID].SetDrawer(value);
    }
    
    [MessageHandler((ushort) MessageID.AllClear)]
    private static void HandleAllClear(Message _)
    {
        Renderer.BeginTextureMode(GameData.Painting!.Value);
        Renderer.ClearBackground(GameData.ClearColor);
        Renderer.EndTextureMode();
    }
    
    [MessageHandler((ushort) MessageID.SendPainting)]
    private static void HandlePainting(Message message)
    {
        Renderer.BeginTextureMode(GameData.Painting!.Value);
        var start = message.GetVector2();
        var end = message.GetVector2();
        var thickness = message.GetFloat();
        var color = message.GetColor();
        Renderer.MouseDrawing(start, end, thickness, color);
        Renderer.EndTextureMode();
    }
    
    #endregion
}

public static class NetworkManager
{
    public static readonly Dictionary<ushort, Player> Players = new();
    
    public static bool IsHost
    {
        get => Server.IsHosted;
        set
        {
            Server.IsHosted = value;
            if (Server.IsHosted) 
                Server.Start();
            else 
                Server.Close();
        }
    }

    public static bool IsClientConnected => Client.IsConnected;
    
    private static class Client 
    {
        public static bool IsConnected;
        
        public static void Init()
        {
            ClientManager.Initialize((_, e) =>
            {
                Players.Remove(e.Id);
            });
        }

        public static void Connect(ConnectionInfo info)
        {
            IsConnected = ClientManager.Connect(info.Ip, (ushort)info.Port);
        }
        
        public static bool IsConnectedToServer => Server.IsHosted || ClientManager.Client!.IsConnected;
    }

    private static class Server 
    {
        public static bool IsHosted;

        public static ConnectionInfo ConnectionInfo = ConnectionInfo.Local;

        public static void Init()
        {
            ServerManager.Initialize(
                (_, e) =>
                {
                    var player = new Player(e.Client.Id, false);
                    Players.Add(e.Client.Id, player);
                    MessageHandlers.SendOtherPlayersInfo(e.Client.Id);
                },
                (_, e) =>
                {
                    Players.Remove(e.Client.Id);
                }
            );
        }

        public static void Start()
        {
            ServerManager.Start((ushort)ConnectionInfo.Port, ConnectionInfo.MaxConnection);
        }
        
        public static void Close()
        {
            ServerManager.Stop();
        }

        public static bool ServerIsRunning() => ServerManager.Server.IsRunning;
    }

    public static void Initialize()
    {
        Server.Init();
        Client.Init();
    }
    
    [Obsolete("Will be deleted", false)]
    public static void StartGame()
    {
        // Send from Client to Server
        var message = Message.Create(MessageSendMode.Reliable, MessageID.StartGame);
        message.AddString(GameLogic.CurrentWord);
        ClientManager.Client!.Send(message);
    }

    public static void StartServer(ConnectionInfo info)
    {
        Server.ConnectionInfo = info;
        IsHost = true;
    }
    
    public static void ConnectToServer(ConnectionInfo info)
    {
        Client.Connect(info);
    }

    public static void Update()
    {
        if (IsHost) ServerManager.Update();
        ClientManager.Update();
    }
}