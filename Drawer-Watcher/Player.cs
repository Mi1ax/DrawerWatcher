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

    private bool _isDrawer;

    public bool IsDrawer 
    {
        get => _isDrawer;
        set
        {
            if (_isDrawer == value) return;
            _isDrawer = value;
            //if (NetworkManager.IsHost)
            MessageHandlers.SendDrawerChanged(ID, value);
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

        foreach (var otherPlayer in NetworkManager.Players.Values)
            ServerManager.Server.Send(
                MessageHandlers.CreateNewConnection(otherPlayer.ID, otherPlayer._isDrawer), 
                ID);
            
        NetworkManager.Players.Add(clientId, this);
        ServerManager.Server.SendToAll(MessageHandlers.CreateNewConnection(ID, _isDrawer));
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