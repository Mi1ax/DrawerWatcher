using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Networking;
using CouscousEngine.rlImGui;
using Drawer_Watcher.Managers;
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

    public void Update()
    {
        if (!IsDrawer || !CanDraw || 
            !(Docking.IsViewportFocused && Docking.IsViewportHovered)) return;

        if (Input.IsMouseButtonDown(MouseButton.LEFT))
        {
            SendDrawingData(Input.GetMousePosition(), CurrentBrush.Color);
        } 
        else if (Input.IsMouseButtonDown(MouseButton.RIGHT))
        {
            SendDrawingData(Input.GetMousePosition(), Brush.ClearColor);
        }
    }

    #region Client

    private void SendDrawingData(Vector2 position, Color color)
    {
        // Send from Client to Server
        if (!_isDrawer) return;
        
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.SendPainting);
        message.AddVector2(position);
        message.AddFloat(CurrentBrush.Thickness);
        message.AddColor(color);
        
        ClientManager.Client!.Send(message);
    }

    [MessageHandler((ushort) MessageID.SendPainting)]
    private static void ReceivePaintingHadler(Message message)
    {
        // Get data from Server to Client
        Renderer.BeginTextureMode(GameData.Painting);
        Renderer.DrawCircle(message.GetVector2(), message.GetFloat(), message.GetColor());
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

    #endregion
}