using System.Numerics;
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
    NewWord,
    Winner,
    
    ConnectionInfo,
    ClientInfo,
    
    DrawerStatus,
    AllClear,
}

public static class NetworkLogger
{
    public static void Init()
    {
        ServerManager.SetLogger(Log);
        ClientManager.SetLogger(Log);
    }

    private static void Log(string fmt)
    {
        Console.WriteLine(fmt);
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
    
    public static void SendMessageInChat(ushort senderID, string text)
    {
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.ChatMessage);
        message.AddUShort(senderID);
        message.AddString(NetworkManager.Players[senderID].Nickname);
        message.AddString(text);
        ClientManager.Client!.Send(message);
    }

    public static void GetNewWord()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.NewWord);
        message.AddString(GameManager.GetRandomWord());
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
            message.AddString(player.Nickname);
            message.AddBool(player.IsDrawer);
        }
        ServerManager.Server.Send(message, toClientId);

        var newPlayer = Message.Create(MessageSendMode.Reliable, MessageID.ConnectionInfo);
        newPlayer.AddInt(1);
        newPlayer.AddUShort(toClientId);
        newPlayer.AddString(NetworkManager.Players[toClientId].Nickname);
        newPlayer.AddBool(false);
        ServerManager.Server.SendToAll(message, toClientId);
    }
    
    [MessageHandler((ushort) MessageID.ClientInfo)]
    private static void HandleClientInfo(ushort fromClientID, Message message)
    {
        Log("(To all Clients) New client info");
        ServerManager.Server.SendToAll(message);
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
    
    [MessageHandler((ushort) MessageID.StartGame)]
    private static void HandleStartGame(ushort fromClientID, Message message)
    {
        ServerManager.Server.SendToAll(message);
    }

    [MessageHandler((ushort) MessageID.ChatMessage)]
    private static void HandleChatMessage(ushort fromClientID, Message message)
    {
        ServerManager.Server.SendToAll(message);
    }
    
    [MessageHandler((ushort) MessageID.Winner)]
    private static void HandleWinner(ushort fromClientID, Message message)
    {
        ServerManager.Server.SendToAll(message, fromClientID);
    }
    
    [MessageHandler((ushort) MessageID.NewWord)]
    private static void HandleNewWord(ushort fromClientID, Message message)
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
            var nickname = message.GetString();
            var player = new Player(id, message.GetBool())
            {
                Nickname = nickname
            };
            if (!NetworkManager.Players.ContainsKey(id))
                NetworkManager.Players.Add(id, player);
        }
    }
    
    [MessageHandler((ushort) MessageID.ClientInfo)]
    private static void HandleNewClientInfo(Message message)
    {
        var clientID = message.GetUShort();
        var clientNick = message.GetString();
        Log($"(From Server) Get nick from {clientID}: {clientNick}");
        NetworkManager.Players[clientID].Nickname = clientNick;
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
    
    [MessageHandler((ushort) MessageID.ChatMessage)]
    private static void HandleChatMessage(Message message)
    {
        var senderID = message.GetUShort();
        var nickname = message.GetString();
        var text = message.GetString();
        if (text == GameManager.CurrentWord)
        {
            GameManager.Guesser = senderID;
            var winnerMessage = Message.Create(MessageSendMode.Reliable, MessageID.Winner);
            winnerMessage.AddUShort(senderID);
            ClientManager.Client!.Send(winnerMessage);
        }

        ChatPanel.AddMessage(nickname, text);
    }
    
    [MessageHandler((ushort) MessageID.Winner)]
    private static void HandleWinner(Message message)
    {
        var senderID = message.GetUShort();
        Log($"(From Server) Winner is {senderID}");
        GameManager.Guesser = senderID;
    }
    
    [MessageHandler((ushort) MessageID.NewWord)]
    private static void HandleNewWord(Message message)
    {
        var newWord = message.GetString();
        Log($"(From Server) Getting new word {newWord}");
        GameManager.CurrentWord = newWord;
        GameManager.Guesser = 0;
    }
    
    [MessageHandler((ushort) MessageID.StartGame)]
    private static void HandleStartGame(Message message)
    {
        Log("(From Server) Starting game");
        ScreenManager.NavigateTo(new GameScreen());
    }
    
    #endregion
}

public static class NetworkManager
{
    public static readonly Dictionary<ushort, Player> Players = new();
    
    public static bool IsHost 
    {
        get => Server.IsHosted;
        private set
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

        private static string _clientNickname = "DefaultNickname";
        
        public static void Init()
        {
            ClientManager.Initialize(OnServerConnected, OnServerDisconnected, OnClientDisconnected);
        }

        private static void OnServerConnected(
            object? sender, 
            EventArgs args)
        {
            var message = Message.Create(MessageSendMode.Reliable, MessageID.ClientInfo);
            message.AddUShort(ClientManager.Client!.Id);
            message.AddString(_clientNickname);
            ClientManager.Client.Send(message);
        }
        
        private static void OnServerDisconnected(
            object? sender, 
            DisconnectedEventArgs args)
        {
            Players.Clear();
            IsConnected = false;
            ScreenManager.NavigateTo(new MenuScreen());
        }
        
        private static void OnClientDisconnected(
            object? sender, 
            ClientDisconnectedEventArgs args)
        {
            Players.Remove(args.Id);
        }

        public static void Connect(ConnectionInfo info, string nickname)
        {
            _clientNickname = nickname;
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
    
    public static void StartGame()
    {
        // Send from Client to Server
        var message = Message.Create(MessageSendMode.Reliable, MessageID.StartGame);
        ClientManager.Client!.Send(message);
    }

    public static void StartServer(ConnectionInfo info)
    {
        Server.ConnectionInfo = info;
        IsHost = true;
    }
    
    public static void ConnectToServer(ConnectionInfo info, string nickname)
    {
        Client.Connect(info, nickname);
    }

    public static void Update()
    {
        if (IsHost) ServerManager.Update();
        ClientManager.Update();
    }
}