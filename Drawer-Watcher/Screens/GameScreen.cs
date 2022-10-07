using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.rlImGui;
using CouscousEngine.Utils;
using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class GameScreen : Screen
{
    public override void OnUpdate()
    {
        if (NetworkManager.IsConnectedToServer && GameManager.Players.Count != 0) 
        {
            foreach (var player in GameManager.Players.Values)
                player.Update();
        }
        
        rlImGui.Begin();
        Docking.Draw(() =>
        {
            var size = ImGui.GetContentRegionAvail();
            var viewRect = new Raylib_CsLo.Rectangle
            {
                x = GameData.Painting.texture.width / 2f - size.X / 2,
                y = GameData.Painting.texture.height / 2f - size.Y / 2,
                width = size.X,
                height = -size.Y
            };
            
            rlImGui.ImageRect(GameData.Painting.texture, (int)size.X, (int)size.Y, viewRect);
        }, () =>
        {
            if (NetworkManager.IsHost)
            {
                ImGui.Begin("Players");
                if (ImGui.Button("Clear All"))
                {
                    Renderer.BeginTextureMode(GameData.Painting);
                    Renderer.ClearBackground(GameData.ClearColor);
                    Renderer.EndTextureMode();
                }
                foreach (var (id, player) in GameManager.Players)
                {
                    ImGui.Text($"Player {player.IsApplicationOwner}: {id}");
                    //player.CanDraw = !ImGui.IsWindowHovered();
                    var isDrawer = player.IsDrawer;
                    ImGui.Checkbox($"Drawer##{id}", ref isDrawer);
                    player.IsDrawer = isDrawer;
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
                Vector3 playerBrushColor = brush.Color;
                ImGui.ColorEdit3("Color", ref playerBrushColor);
                Player.ApplicationOwner.CurrentBrush.Color = (Color)playerBrushColor;
                
                // Thickness
                var thickness = brush.Thickness;
                ImGui.DragFloat("Thickness", ref thickness, 1f, 2f, 32f);
                Player.ApplicationOwner.CurrentBrush.Thickness = thickness;
                
                ImGui.End();
            }
        });
        rlImGui.End();
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}