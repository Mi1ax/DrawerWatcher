using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Networking;
using Riptide;
using Color = CouscousEngine.Utils.Color;
using MouseButton = CouscousEngine.Core.MouseButton;

namespace Drawer_Watcher;

public class Player
{
    public ushort ID { get; }

    public Player(ushort clientId)
    {
        ID = clientId;
        
        GameManager.Players.Add(clientId, this);
    }

    public void Update()
    {
        
        if (Input.IsMouseButtonDown(MouseButton.LEFT))
        {
            SendDrawing(Input.GetMousePosition(), Color.RED);
        } 
        else if (Input.IsMouseButtonDown(MouseButton.RIGHT))
        {
            SendDrawing(Input.GetMousePosition(), GameData.ClearColor);
        }
    }

    private static void SendDrawing(Vector2 position, Color color)
    {
        var message = Message.Create(MessageSendMode.Unreliable, MessageID.SendPainting);
        message.AddVector2(position);
        message.AddColor(color);
        
        ClientManager.Client!.Send(message);
    }
}