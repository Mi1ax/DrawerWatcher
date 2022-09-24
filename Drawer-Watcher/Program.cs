using System.Diagnostics;
using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Networking;
using CouscousEngine.Utils;
using Drawer_Watcher.Screens;
using _rl = Raylib_CsLo;
using _gui = Raylib_CsLo.RayGui;
using Color = CouscousEngine.Utils.Color;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace Drawer_Watcher;

internal class Sandbox : Application
{
    private readonly Rectangle _checkboxPlayers;

    public Sandbox() 
        : base("Sandbox")
    {
        _checkboxPlayers = new Rectangle(
            new Size(25, 25), 
            new Vector2(Window.Width - 125, 45), Color.WHITE);

        ScreenManager.NavigateTo(new ConnectionScreen());
            
        GameManager.Initialize();
    }
    
    protected override void Update()
    {
        GameManager.Update();
        
        Renderer.BeginDrawing();
        Renderer.ClearBackground(GameData.ClearColor);
        {
            GameManager.DrawPainting();
            
            ScreenManager.Update();

            if (!GameManager.IsConnectedToServer)
            {
                if (false)
                {
                    GameManager.Connect();
                }

                if (false)
                {
                    GameManager.IsHost = true;
                    GameManager.Connect();
                }
            }

            if (GameManager.IsConnectedToServer && GameManager.Players.Count != 0) 
            {
                _rl.Raylib.DrawText($"Client [{ClientManager.Client!.Id}]; " +
                                    $"Drawer [{GameManager.Players[ClientManager.Client.Id].IsDrawer}]", 
                    15, Window.Height - 78f, 24f, Color.GREEN);

                foreach (var player in GameManager.Players.Values)
                    player.Update();
            } 
            if (GameManager.IsHost) 
            {
                _rl.Raylib.DrawText($"Host [{GameManager.Players.Count}]", 15, Window.Height - 48f, 24f, Color.RED);

                foreach (var (id, player) in GameManager.Players)
                {
                    var copy = _checkboxPlayers.Clone() as Rectangle;
                    copy!.Position.Y = _checkboxPlayers.Position.Y + _checkboxPlayers.Size.Height * id + 10;
                    
                    if (!_gui.GuiCheckBox(copy, "Drawer", player.IsDrawer)) continue;
                    foreach (var otherPlayers in GameManager.Players.Values)
                        otherPlayers.IsDrawer = false;

                    GameManager.Players[id].IsDrawer = true;
                }
            }
            
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