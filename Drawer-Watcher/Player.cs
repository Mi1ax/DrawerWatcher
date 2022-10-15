using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Networking;
using Drawer_Watcher.Managers;
using Color = CouscousEngine.Utils.Color;
using MouseButton = CouscousEngine.Core.MouseButton;

namespace Drawer_Watcher;

public class Player
{
    public static Player? ApplicationOwner;
    
    public ushort ID { get; }

    public bool IsDrawer { get; private set; }

    public void SetDrawerWithNotifyngServer(bool value)
    {
        IsDrawer = value;
        MessageHandlers.SetDrawer(ID, value);
    }

    public void SetDrawer(bool value)
        => IsDrawer = value;

    public bool CanDraw = true;
    public bool IsApplicationOwner => ClientManager.Client?.Id == ID;

    public Brush CurrentBrush;
    
    private Vector2 _prevPoint = Vector2.Zero;
    private Vector2 _currPoint = Vector2.Zero;

    public Player(ushort clientId, bool isDrawer)
    {
        ID = clientId;
        IsDrawer = isDrawer;
        if (IsApplicationOwner)
            ApplicationOwner = this;
        CurrentBrush = Brush.Default;
    }

    public void Update()
    {
        if (!IsDrawer || !CanDraw) return;

        _currPoint = Input.GetMousePosition();
        
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