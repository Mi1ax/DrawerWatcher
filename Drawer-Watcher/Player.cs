using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Networking;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Screens;
using ImGuiNET;
using Color = CouscousEngine.Utils.Color;
using MouseButton = CouscousEngine.Core.MouseButton;

namespace Drawer_Watcher;

public class Player
{
    public static Player? ApplicationOwner => 
        ClientManager.Client == null 
            ? null 
            : NetworkManager.Players[ClientManager.Client.Id];

    public bool IsAppOwner => ApplicationOwner!.ID == ID;

    public ushort ID { get; }
    public int Score { get; set; }

    public bool IsDrawer { get; private set; }
    public bool IsInLobby { get; set; }
    
    public string Nickname { get; set; }
    
    public void SetDrawerWithNotifyngServer(bool value)
    {
        IsDrawer = value;
        MessageHandlers.SetDrawer(ID, value);
    }

    public void SetDrawer(bool value)
        => IsDrawer = value;

    public Brush CurrentBrush;
    
    private Vector2 _prevPoint = Vector2.Zero;
    private Vector2 _currPoint = Vector2.Zero;

    public Player(ushort clientId, bool isDrawer)
    {
        ID = clientId;
        IsDrawer = isDrawer;
        CurrentBrush = Brush.Default;
        Nickname = "DefaultNickname";
    }

    public void Update(bool isViewport)
    {
        if (!isViewport)
        {
            _currPoint = Vector2.Zero;
            _prevPoint = Vector2.Zero;
            return;
        }
        if (!IsDrawer || !IsAppOwner) return;

        _currPoint = Input.GetMousePosition() - GameScreen.CursorOffset;
        if (_prevPoint == Vector2.Zero)
            _prevPoint = _currPoint;
        
        if (Input.IsMouseButtonPressed(MouseButton.LEFT))
        {
            MessageHandlers.SendDrawingData(_currPoint, _currPoint, CurrentBrush.Thickness, (Color)CurrentBrush.Color);
        } 
        else if (Input.IsMouseButtonPressed(MouseButton.RIGHT))
        {
            MessageHandlers.SendDrawingData(_currPoint, _currPoint, CurrentBrush.Thickness, Brush.ClearColor);
        }
        
        if (Input.IsMouseButtonDown(MouseButton.LEFT))
        {
            MessageHandlers.SendDrawingData(_prevPoint, _currPoint, CurrentBrush.Thickness, (Color)CurrentBrush.Color);
        } 
        else if (Input.IsMouseButtonDown(MouseButton.RIGHT))
        {
            MessageHandlers.SendDrawingData(_prevPoint, _currPoint, CurrentBrush.Thickness, Brush.ClearColor);
        }

        _prevPoint = _currPoint;
    }
}