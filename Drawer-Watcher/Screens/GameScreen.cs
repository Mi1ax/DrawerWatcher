using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Shapes;
using CouscousEngine.Utils;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Panels;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class GameScreen : Screen
{
    public static Vector2 MousePositionOnPainting = Vector2.Zero;
    
    private readonly Rectangle _drawingPanel;
    private readonly ChatPanel _chatPanel;
    private readonly ToolPanel _toolPanel;

    public GameScreen()
    {
        _chatPanel = new ChatPanel(new Rectangle(new Size(350, 720), Vector2.Zero)
        {
            Color = new Color(193, 193, 193)
        });
        
        _toolPanel = new ToolPanel(new Rectangle(new Size(930, 144), new Vector2(350, 720 - 144))
        {
            Color = Color.GRAY
        });
        
        _drawingPanel = new Rectangle(new Size(930, 720 - 144), new Vector2(350, 0));

        GameData.Painting = Renderer.LoadRenderTexture((int)_drawingPanel.Size.Width, (int)_drawingPanel.Size.Height);
    }

    public override void OnUpdate()
    {
        if (!NetworkManager.IsConnectedToServer || GameManager.Players.Count == 0) return;
        MousePositionOnPainting = Input.GetMousePosition() - new Vector2(350, 0);
            
        foreach (var player in GameManager.Players.Values)
            player.Update();
            
        Renderer.DrawTexture(GameData.Painting!.Value, _drawingPanel.Position, Color.WHITE);
        Renderer.DrawRectangleLines(_drawingPanel, 1f, Color.BLACK);
        
        // TODO: Move somewhere
        _rl.DrawLine(351, 720 - 144, 350, 720, Color.BLACK);

        _chatPanel.OnUpdate();
        _toolPanel.OnUpdate();
    }

    public override void OnImGuiUpdate()
    {
        if (NetworkManager.IsHost)
        {
            ImGui.Begin("Players");
            if (ImGui.Button("Clear all"))
                Player.SendAllClear();
            
            foreach (var (id, player) in GameManager.Players)
            {
                ImGui.Text($"Player {player.IsApplicationOwner}: {id}");
                var isDrawer = player.IsDrawer;
                ImGui.Checkbox($"Drawer##{id}", ref isDrawer);
                if (player.IsDrawer != isDrawer)
                {
                    foreach (var otherPlayers in GameManager.Players.Values)
                        otherPlayers.IsDrawer = false;
                    player.IsDrawer = isDrawer;
                }
                ImGui.Separator();
            }
            ImGui.End();
        }

        if (Player.ApplicationOwner != null)
        {
            var brush = Player.ApplicationOwner.CurrentBrush;
            
            ImGui.Begin($"Player: {Player.ApplicationOwner.ID}");
            ImGui.Text("Brush");
            // Color
            var playerBrushColor = brush.Color;
            ImGui.ColorEdit3("Color", ref playerBrushColor);
            if (playerBrushColor != brush.Color)
                Player.ApplicationOwner.CurrentBrush.Color = playerBrushColor;
            
            // Thickness
            var thickness = brush.Thickness;
            ImGui.DragFloat("Thickness", ref thickness, 1f, 2f, 32f);
            Player.ApplicationOwner.CurrentBrush.Thickness = thickness;
            
            ImGui.End();
        }
    }

    public override void Dispose()
    {
        _toolPanel.Dispose();
        _chatPanel.Dispose();
        GC.SuppressFinalize(this);
    }
}