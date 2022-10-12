using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Networking;
using CouscousEngine.rlImGui;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Panels;
using Drawer_Watcher.Screens;
using ImGuiNET;
using Riptide;
using Color = CouscousEngine.Utils.Color;
using MouseButton = CouscousEngine.Core.MouseButton;

namespace Drawer_Watcher;

public class Player
{
    public static Player? ApplicationOwner;
    
    public ushort ID { get; }

    private bool _isDrawer;

    public bool IsDrawer 
    {
        get => _isDrawer;
        set
        {
            if (_isDrawer == value) return;
            _isDrawer = value;
            if (NetworkManager.IsHost)
                SendDrawerChanged(value);
        }
    }

    public bool CanDraw = true;
    public bool IsApplicationOwner => ClientManager.Client?.Id == ID;

    public Brush CurrentBrush;
    
    private Vector2 _prevPoint = Vector2.Zero;
    private Vector2 _currPoint = Vector2.Zero;

    public Player(ushort clientId)
    {
        ID = clientId;
        if (IsApplicationOwner)
            ApplicationOwner = this;
        CurrentBrush = Brush.Default;

        if (!NetworkManager.IsHost) return;

        foreach (var otherPlayer in GameManager.Players.Values)
            ServerManager.Server.Send(otherPlayer.CreateNewConnectionMessage(), ID);
            
        GameManager.Players.Add(clientId, this);
        ServerManager.Server.SendToAll(CreateNewConnectionMessage());
    }

    private static void DrawLine(Vector2 start, Vector2 end, float thickness, Color color)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var distance = Math.Max(Math.Abs(dx), Math.Abs(dy));
        for (var i = 0; i < distance; i++)
        {
            var x = (int)(start.X + i / distance * dx);
            var y = (int)(start.Y + i / distance * dy);
            _rl.DrawCircleV(new Vector2(x, y), thickness, color);
        }
    }
    
    public void Update()
    {
        if (!IsDrawer || !CanDraw) return;

        _currPoint = GameScreen.MousePositionOnPainting;
        
        if (Input.IsMouseButtonDown(MouseButton.LEFT))
        {
            SendDrawingData(_prevPoint, _currPoint, (Color)CurrentBrush.Color);
        } 
        else if (Input.IsMouseButtonDown(MouseButton.RIGHT))
        {
            SendDrawingData(_prevPoint, _currPoint, Brush.ClearColor);
        }

        _prevPoint = _currPoint;
    }

    #region Client

    private void SendDrawingData(Vector2 start, Vector2 end, Color color)
    {
        // Send from Client to Server
        if (!_isDrawer) return;
        if (ImGui.GetIO().WantCaptureMouse) return;
        
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.SendPainting);
        message.AddVector2(start);
        message.AddVector2(end);
        message.AddFloat(CurrentBrush.Thickness);
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
        ClientManager.Client!.Send(message);
    }
    
    [MessageHandler((ushort) MessageID.ChatMessage)]
    private static void ReceiveChatMessageHadler(Message message)
    {
        // Get data from Server to Client
        var senderID = message.GetUShort();
        var text = message.GetString();
        ChatPanel.AddMessage(senderID, text);
    }

    [MessageHandler((ushort) MessageID.SendPainting)]
    private static void ReceivePaintingHadler(Message message)
    {
        // Get data from Server to Client
        Renderer.BeginTextureMode(GameData.Painting!.Value);
        DrawLine(message.GetVector2(), message.GetVector2(), message.GetFloat(), message.GetColor());
        //Renderer.DrawCircle(message.GetVector2(), message.GetFloat(), message.GetColor());
        Renderer.EndTextureMode();
    }
    
    [MessageHandler((ushort) MessageID.AllClear)]
    private static void ReceiveAllClearHadler(Message _)
    {
        // Get data from Server to Client
        Renderer.BeginTextureMode(GameData.Painting!.Value);
        Renderer.ClearBackground(GameData.ClearColor);
        Renderer.EndTextureMode();
    }

    [MessageHandler((ushort) MessageID.SendPosition)]
    private static void ReceiveNewConnnectionHandler(Message message)
    {
        // Get data from Server to Client
        if (NetworkManager.IsHost) return;
        
        var id = message.GetUShort();
        var isDrawer = message.GetBool();

        GameManager.Players.Add(id, new Player(id) { IsDrawer = isDrawer });
    }
    
    [MessageHandler((ushort) MessageID.DrawerChanged)]
    private static void ReceiveDrawerChangedHandler(Message message)
    {
        // Get data from Server to Client
        
        var id = message.GetUShort();
        var isDrawer = message.GetBool();

        GameManager.Players[id]._isDrawer = isDrawer;
    }

    #endregion
    
    #region Server

    private void SendDrawerChanged(bool value)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.DrawerChanged);
        message.AddUShort(ID);
        message.AddBool(value);
        ServerManager.Server.SendToAll(message);
    }
    
    private Message CreateNewConnectionMessage()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.SendPosition);
        message.AddUShort(ID);
        message.AddBool(IsDrawer);
        return message;
    }
    
    [MessageHandler((ushort) MessageID.SendPainting)]
    private static void ReceivePaintingHandler(ushort fromClientID, Message message)
    {
        if (!GameManager.Players[fromClientID]._isDrawer) return;
        // Receive data from Client and use in Server 
        ServerManager.Server.SendToAll(message);
    }
    
    
    [MessageHandler((ushort) MessageID.AllClear)]
    private static void ReceiveAllClearHandler(ushort fromClientID, Message message)
    {
        if (!GameManager.Players[fromClientID]._isDrawer) return;
        // Receive data from Client and use in Server 
        ServerManager.Server.SendToAll(message);
    }
    
    [MessageHandler((ushort) MessageID.ChatMessage)]
    private static void ReceiveChatMessageHandler(ushort fromClientID, Message message)
    {
        // Receive data from Client and use in Server 
        ServerManager.Server.SendToAll(message);
    }

    #endregion
}