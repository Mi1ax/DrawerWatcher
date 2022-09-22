using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Networking;
using CouscousEngine.Utils;
using _rl = Raylib_CsLo;
using _gui = Raylib_CsLo.RayGui;
using Color = CouscousEngine.Utils.Color;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace Drawer_Watcher;

internal class Sandbox : Application
{
    private readonly Rectangle _clientButton;
    private readonly Rectangle _serverButton;

    public Sandbox() 
        : base("Sandbox")
    {
        _clientButton = new Rectangle(
            new Size(125, 25),
            new Vector2(Window.Width / 2f - 125 / 2f, 155), Color.WHITE);
        
        _serverButton = new Rectangle(
            new Size(125, 25),
            new Vector2(Window.Width / 2f - 125 / 2f, 185), Color.WHITE);
        
        GameManager.Initialize();
    }
    
    protected override void Update()
    {
        
    }

    protected override void Draw()
    {
        GameManager.Update();
        
        Renderer.BeginDrawing();
        Renderer.ClearBackground(GameData.ClearColor);
        {
            if (!GameManager.IsConnectedToServer)
            {
                if (_gui.GuiButton(_clientButton, "Connect"))
                {
                    var test = GameManager.Connect();
                }

                if (_gui.GuiButton(_serverButton, "Create Game"))
                {
                    GameManager.IsHost = true;
                    GameManager.Connect();
                }
            }

            if (GameManager.IsConnectedToServer)
            {
                _rl.Raylib.DrawText($"Client [{ClientManager.Client!.Id}]", 15, Window.Height - 78f, 24f, Color.GREEN);

                foreach (var player in GameManager.Players.Values)
                    player.Update();
            } 
            if (GameManager.IsHost)
            {
                _rl.Raylib.DrawText($"Host [{GameManager.Players.Count}]", 15, Window.Height - 48f, 24f, Color.RED);
            }
            
            GameManager.DrawPainting();

            Renderer.DrawFPS();
        }
        Renderer.EndDrawing();
    }
}

internal static class Program
{
    private static void Main()
    {
        using var app = new Sandbox();
        app.Run();
    }
}