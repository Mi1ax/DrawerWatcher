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
    SendPosition,
    DrawerChanged,
    AllClear,
    
    ChatMessage,
    
    StartGame,
    SendWinning
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
        
        #region Receive data from Server and handle on Client side

        [MessageHandler((ushort) MessageID.SendPainting)]
        private static void ReceivePainting(Message message)
        {
            Log(nameof(ReceivePainting), nameof(MessageID.SendPainting));
            Renderer.BeginTextureMode(GameData.Painting!.Value);
            Renderer.MouseDrawing(message.GetVector2(), message.GetVector2(), message.GetFloat(), message.GetColor());
            //Renderer.DrawCircle(message.GetVector2(), message.GetFloat(), message.GetColor());
            Renderer.EndTextureMode();
        }
        
        [MessageHandler((ushort) MessageID.SendPosition)]
        private static void ReceiveNewConnnection(Message message)
        {
            if (NetworkManager.IsHost) return;
            Log(nameof(ReceiveNewConnnection), nameof(MessageID.SendPosition));

            var id = message.GetUShort();
            var isDrawer = message.GetBool();

            NetworkManager.Players.Add(id, new Player(id) { IsDrawer = isDrawer });
        }
        
        [MessageHandler((ushort) MessageID.DrawerChanged)]
        private static void ReceiveDrawerChanged(Message message)
        {
            Log(nameof(ReceiveDrawerChanged), nameof(MessageID.DrawerChanged));

            var id = message.GetUShort();
            var isDrawer = message.GetBool();

            NetworkManager.Players[id].IsDrawer = isDrawer;
        }
        
        [MessageHandler((ushort) MessageID.AllClear)]
        private static void ReceiveAllClear(Message _)
        {
            Log(nameof(ReceiveAllClear), nameof(MessageID.AllClear));
            Renderer.BeginTextureMode(GameData.Painting!.Value);
            Renderer.ClearBackground(GameData.ClearColor);
            Renderer.EndTextureMode();
        }
        
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

        [MessageHandler((ushort) MessageID.SendPainting)]
        private static void ReceivePainting(ushort fromClientID, Message message)
        {
            Log(nameof(ReceivePainting), nameof(MessageID.SendPainting));
            if (!NetworkManager.Players[fromClientID].IsDrawer) return;
            ServerManager.Server.SendToAll(message);
        }
        
        [MessageHandler((ushort) MessageID.DrawerChanged)]
        private static void ReceiveDrawerChanged(ushort fromClientID, Message message)
        {
            Log(nameof(ReceiveDrawerChanged), nameof(MessageID.DrawerChanged));
            ServerManager.Server.SendToAll(message);
        }

        [MessageHandler((ushort) MessageID.StartGame)]
        private static void ReceiveStartGame(ushort fromClientID, Message message)
        {
            Log(nameof(ReceiveStartGame), nameof(MessageID.StartGame));
            ServerManager.Server.SendToAll(message);
        }
    
    
        [MessageHandler((ushort) MessageID.AllClear)]
        private static void ReceiveAllClear(ushort fromClientID, Message message)
        {
            if (!NetworkManager.Players[fromClientID].IsDrawer) return;
            Log(nameof(ReceiveAllClear), nameof(MessageID.AllClear));
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

        public static Message CreateNewConnection(ushort id, bool isDrawer)
        {
            Log(nameof(CreateNewConnection), nameof(MessageID.SendPosition));
            var message = Message.Create(MessageSendMode.Reliable, MessageID.SendPosition);
            message.AddUShort(id);
            message.AddBool(isDrawer);
            return message;
        }

        public static void SendDrawerChanged(ushort id, bool value)
        {
            var message = Message.Create(MessageSendMode.Reliable, MessageID.DrawerChanged);
            message.AddUShort(id);
            message.AddBool(value);
            //ServerManager.Server.SendToAll(message);
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

        public static void SendAllClear()
        {
            var message = Message.Create(MessageSendMode.Unreliable, MessageID.AllClear);
            ClientManager.Client!.Send(message);
        }

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
                    var unused = new Player(e.Client.Id)
                    {
                        IsDrawer = false,
                        CanDraw = false,
                        CurrentBrush = default
                    };
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