using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Networking;
using CouscousEngine.Utils;
using Drawer_Watcher.Panels;
using Drawer_Watcher.Screens;
using Drawer_Watcher.Screens.ImGuiWindows;
using ImGuiNET;
using Riptide;
using Riptide.Utils;

namespace Drawer_Watcher.Managers;

public enum MessageId : ushort
{
    SendPainting = 1,
    ChatMessage,
    SameNick,
    StartGame,
    LobbyExit,
    NewWord,
    Timer,
    TimesUp,
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

    public static void SetDrawer(ushort clientId, bool value)
    {
        Log("(To Server) New drawer value");
        NetworkManager.Players[clientId].SetDrawer(value);
        var message = Message.Create(MessageSendMode.Reliable, MessageId.DrawerStatus);
        message.AddUShort(clientId);
        message.AddBool(value);
        ClientManager.Client!.Send(message);
    }

    public static void SendLobbyExit()
    {
        Log("(To Server) Exit to lobby");
        ClientManager.Client!.Send(Message.Create(MessageSendMode.Reliable, MessageId.LobbyExit));
    }
    
    public static void ClearPainting()
    {
        Log("(To Server) Want to clear painting");
        var message = Message.Create(MessageSendMode.Unreliable, MessageId.AllClear);
        ClientManager.Client!.Send(message);
    }
    
    public static void SendDrawingData(Vector2 start, Vector2 end, float thickness, Color color)
    {
        var message = Message.Create(MessageSendMode.Unreliable, MessageId.SendPainting);
        message.AddVector2(start);
        message.AddVector2(end);
        message.AddFloat(thickness);
        message.AddColor(color);
        
        ClientManager.Client!.Send(message);
    }
    
    public static void SendMessageInChat(ushort senderId, string text)
    {
        var message = Message.Create(MessageSendMode.Unreliable, MessageId.ChatMessage);
        message.AddUShort(senderId);
        message.AddString(NetworkManager.Players[senderId].Nickname);
        message.AddString(text);
        ClientManager.Client!.Send(message);
    }

    public static void SendNewWord()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageId.NewWord);
        message.AddString(GameManager.GetRandomWord());
        ClientManager.Client!.Send(message);
    }

    public static void SendTime(string time)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageId.Timer);
        message.AddString(time);
        ClientManager.Client!.Send(message);
    }
    
    public static void SendTimesUp()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageId.TimesUp);
        ClientManager.Client!.Send(message);
    }

    #endregion

    #region Send data from Server to the Client/Clients

    public static void SendOtherPlayersInfo(ushort toClientId)
    {
        Log($"(To Client) Send {toClientId} other players info");
        var message = Message.Create(MessageSendMode.Reliable, MessageId.ConnectionInfo);
        message.AddBool(GameManager.IsGameStarted);
        message.AddInt(NetworkManager.Players.Count);
        foreach (var (id, player) in NetworkManager.Players)
        {
            message.AddUShort(id);
            message.AddString(player.Nickname);
            message.AddBool(player.IsDrawer);
            message.AddBool(player.IsInLobby);
        }
        ServerManager.Server.Send(message, toClientId);

        var newPlayer = Message.Create(MessageSendMode.Reliable, MessageId.ConnectionInfo);
        newPlayer.AddBool(GameManager.IsGameStarted);
        newPlayer.AddInt(1);
        newPlayer.AddUShort(toClientId);
        newPlayer.AddString(NetworkManager.Players[toClientId].Nickname);
        newPlayer.AddBool(false);
        newPlayer.AddBool(true);
        ServerManager.Server.SendToAll(message, toClientId);
    }
    
    [MessageHandler((ushort) MessageId.ClientInfo)]
    private static void HandleClientInfo(ushort fromClientId, Message message)
    {
        Log("(To all Clients) New client info");
        var clientId = message.GetUShort();
        var nickname = message.GetString();
        if (NetworkManager.Players.Values.FirstOrDefault(p => p.Nickname == nickname) != null)
        {
            var disconnectMessage = Message.Create(MessageSendMode.Reliable, MessageId.SameNick);
            disconnectMessage.AddUShort((ushort)MessageId.SameNick);
            ServerManager.Server.DisconnectClient(clientId, disconnectMessage);
        } else
            ServerManager.Server.SendToAll(message);
    }
    
    [MessageHandler((ushort) MessageId.DrawerStatus)]
    private static void HandleDrawerMessage(ushort fromClientId, Message message)
    {
        Log("(To all Clients) Drawer status");
        ServerManager.Server.SendToAll(message, fromClientId);
    }
    
    [MessageHandler((ushort) MessageId.LobbyExit)]
    private static void HandleLobbyExit(ushort fromClientId, Message message)
    {
        Log("(To all Clients) Exit to lobby");
        foreach (var (id, player) in NetworkManager.Players)
        {
            if (!player.IsInLobby && id != fromClientId)
                ServerManager.Server.Send(message, id);
        }
    }
    
    [MessageHandler((ushort) MessageId.AllClear)]
    private static void HandleAllClear(ushort fromClientId, Message message)
    {
        if (!NetworkManager.Players[fromClientId].IsDrawer) return;
        Log("(To all Clients) Clear painting");
        foreach (var (id, player) in NetworkManager.Players)
        {
            if (!player.IsInLobby)
                ServerManager.Server.Send(message, id);
        }
    }
    
    [MessageHandler((ushort) MessageId.SendPainting)]
    private static void HandlePainting(ushort fromClientId, Message message)
    {
        foreach (var (id, player) in NetworkManager.Players)
        {
            if (!player.IsInLobby)
                ServerManager.Server.Send(message, id);
        }
    }
    
    [MessageHandler((ushort) MessageId.StartGame)]
    private static void HandleStartGame(ushort fromClientId, Message message)
    {
        ServerManager.Server.SendToAll(message);
    }

    [MessageHandler((ushort) MessageId.ChatMessage)]
    private static void HandleChatMessage(ushort fromClientId, Message message)
    {
        foreach (var (id, player) in NetworkManager.Players)
        {
            if (!player.IsInLobby)
                ServerManager.Server.Send(message, id);
        }
    }
    
    [MessageHandler((ushort) MessageId.Winner)]
    private static void HandleWinner(ushort fromClientId, Message message)
    {
        foreach (var (id, player) in NetworkManager.Players)
        {
            if (!player.IsInLobby && id != fromClientId)
                ServerManager.Server.Send(message, id);
        }
    }
    
    [MessageHandler((ushort) MessageId.NewWord)]
    private static void HandleNewWord(ushort fromClientId, Message message)
    {
        foreach (var (id, player) in NetworkManager.Players)
        {
            if (!player.IsInLobby)
                ServerManager.Server.Send(message, id);
        }
    }
    
    [MessageHandler((ushort) MessageId.Timer)]
    private static void HandleTimer(ushort fromClientId, Message message)
    {
        foreach (var (id, player) in NetworkManager.Players)
        {
            if (!player.IsInLobby)
                ServerManager.Server.Send(message, id);
        }
    }
    
    [MessageHandler((ushort) MessageId.TimesUp)]
    private static void HandleTimesUp(ushort fromClientId, Message message)
    {
        foreach (var (id, player) in NetworkManager.Players)
        {
            if (!player.IsInLobby)
                ServerManager.Server.Send(message, id);
        }
    }

    #endregion

    #region Handle Server messages on Client Side

    [MessageHandler((ushort)MessageId.ConnectionInfo)]
    private static void HandleConnectionInfo(Message message)
    {
        GameManager.IsGameStarted = message.GetBool();
        var playersCount = message.GetInt();
        Log($"(From Server) Get data about {playersCount} other players!");
        for (var _ = 0; _ < playersCount; _++)
        {
            var id = message.GetUShort();
            var nickname = message.GetString();
            var player = new Player(id, message.GetBool())
            {
                Nickname = nickname,
                IsInLobby = message.GetBool()
            };
            if (!NetworkManager.Players.ContainsKey(id))
                NetworkManager.Players.Add(id, player);
            else
                NetworkManager.Players[id] = player;
        }
    }
    
    [MessageHandler((ushort) MessageId.ClientInfo)]
    private static void HandleNewClientInfo(Message message)
    {
        var clientId = message.GetUShort();
        var clientNick = message.GetString();
        Log($"(From Server) Get nick from {clientId}: {clientNick}");
        NetworkManager.Players[clientId].Nickname = clientNick;
    }
    
    [MessageHandler((ushort) MessageId.DrawerStatus)]
    private static void HandleDrawerStatus(Message message)
    {
        var clientId = message.GetUShort();
        var value = message.GetBool();
        var msg = value ? "is drawer" : "is not a drawer";
        Log($"(From Server) {clientId}: {msg}");
        NetworkManager.Players[clientId].SetDrawer(value);
    }
    
    [MessageHandler((ushort) MessageId.AllClear)]
    private static void HandleAllClear(Message _)
    {
        NetworkManager.IsWantToClear = true;
        /*Renderer.BeginTextureMode(GameData.Painting!.Value);
        Renderer.ClearBackground(GameData.ClearColor);
        Renderer.EndTextureMode();*/
    }
    
    [MessageHandler((ushort) MessageId.SendPainting)]
    private static void HandlePainting(Message message)
    {
        //Renderer.BeginTextureMode(GameData.Painting!.Value);
        NetworkManager.StartPos = message.GetVector2();
        NetworkManager.EndPos = message.GetVector2();
        NetworkManager.Thickness = message.GetFloat();
        NetworkManager.Color = message.GetColor();
        /*Renderer.MouseDrawing(start, end, thickness, color);
        Renderer.EndTextureMode();*/
    }
    
    [MessageHandler((ushort) MessageId.ChatMessage)]
    private static void HandleChatMessage(Message message)
    {
        var senderId = message.GetUShort();
        var nickname = message.GetString();
        var text = message.GetString();
        if (text == GameManager.CurrentWord)
        {
            GameManager.Guesser = senderId;
            var winnerMessage = Message.Create(MessageSendMode.Reliable, MessageId.Winner);
            winnerMessage.AddUShort(senderId);
            ClientManager.Client!.Send(winnerMessage);
        }

        ChatPanel.AddMessage(nickname, text);
    }
    
    [MessageHandler((ushort) MessageId.Winner)]
    private static void HandleWinner(Message message)
    {
        var senderId = message.GetUShort();
        Log($"(From Server) Winner is {senderId}");
        GameManager.Guesser = senderId;
        // TODO: Time score multiplayer
        NetworkManager.Players[senderId].Score++;
    }
    
    [MessageHandler((ushort) MessageId.NewWord)]
    private static void HandleNewWord(Message message)
    {
        var newWord = message.GetString();
        Log($"(From Server) Getting new word {newWord}");
        GameManager.CurrentWord = newWord;
        GameManager.Guesser = 0;
        GameManager.IsRoundEnded = false;
    }
    
    [MessageHandler((ushort) MessageId.Timer)]
    private static void HandleTimer(Message message)
    {
        var time = message.GetString();
        GameManager.Timer.CurrentTime = time;
    }
    
    [MessageHandler((ushort) MessageId.TimesUp)]
    private static void HandleTimesUp(Message message)
    {
        GameManager.IsRoundEnded = true;
    }
    
    [MessageHandler((ushort) MessageId.StartGame)]
    private static void HandleStartGame(Message message)
    {
        Log("(From Server) Starting game");
        GameManager.IsGameStarted = true;
        foreach (var player in NetworkManager.Players.Values)
            player.IsInLobby = false;
        ScreenManager.NavigateTo(new GameScreen(message.GetInt()));
    }
    
    [MessageHandler((ushort) MessageId.LobbyExit)]
    private static void HandleLobbyExit(Message message)
    {
        Log("(From Server) Exit to lobby");
        GameManager.IsGameStarted = false;
        foreach (var player in NetworkManager.Players.Values)
            player.IsInLobby = true;
        ScreenManager.NavigateTo(new MenuScreen());
    }
    
    #endregion
}

public static class NetworkManager 
{
    public static readonly Dictionary<ushort, Player> Players = new();
 
    public static bool IsWantToClear;
    public static Vector2? StartPos;
    public static Vector2? EndPos;
    public static float? Thickness;
    public static Color? Color;
    
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
    public static bool IsServerRunning => Server.IsRunning;

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
            var message = Message.Create(MessageSendMode.Reliable, MessageId.ClientInfo);
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
            if (args.Message != null &&
                args.Message.GetUShort() == (ushort)MessageId.SameNick)
                MessageBox.Show("Error", "Player with the same nickname already exist", MessageBoxButtons.Ok,
                    result =>
                    {
                        if (result == MessageBoxResult.Ok)  
                            LobbyWindow.IsVisible = false;
                    });
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
                    var player = new Player(e.Client.Id, false)
                    {
                        IsInLobby = true
                    };
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

        public static bool IsRunning => ServerManager.Server.IsRunning;
    }

    public static void Initialize()
    {
        Server.Init();
        Client.Init();
    }
    
    public static void StartGame(int minutes)
    {
        // Send from Client to Server
        var message = Message.Create(MessageSendMode.Reliable, MessageId.StartGame);
        message.AddInt(minutes);
        ClientManager.Client!.Send(message);
    }

    public static void StartServer(ConnectionInfo info)
    {
        Server.ConnectionInfo = info;
        IsHost = true;
        if (!ServerThread.IsAlive && !ServerThread.ThreadState.HasFlag(ThreadState.Stopped)) 
            ServerThread.Start();
        else
        {
            ServerThread = new Thread(ServerUpdate);
            ServerThread.Start();
        }
    }
    
    public static void ConnectToServer(ConnectionInfo info, string nickname)
    {
        if (!ClientThread.IsAlive && !ClientThread.ThreadState.HasFlag(ThreadState.Stopped)) 
            ClientThread.Start();
        else
        {
            ClientThread = new Thread(ClientUpdate);
            ClientThread.Start();
        }
        Client.Connect(info, nickname);
    }

    public static Thread ServerThread = new(ServerUpdate);
    public static Thread ClientThread = new(ClientUpdate);
    
    private static void ServerUpdate()
    {
        while (IsServerRunning)
        {
            if (!IsHost) continue;
            
            ServerManager.Update();
        }
    }
    
    private static void ClientUpdate()
    {
        while (Application.Instance.IsRunning)
        {
            ClientManager.Update();
        }
    }

    public static void CloseConnections()
    {
        ServerManager.Stop();
    }
}