using System.Numerics;
using CouscousEngine.rlImGui;
using CouscousEngine.Shapes;
using CouscousEngine.Utils;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class GameScreen : Screen
{
    public override void OnUpdate()
    {
        if (GameManager.IsConnectedToServer && GameManager.Players.Count != 0) 
        {
            foreach (var player in GameManager.Players.Values)
                player.Update();
        }
        
        if (GameManager.IsHost)
        {
            rlImGui.OnUpdate(() =>
            {
                ImGui.Begin("Players");
                foreach (var (id, player) in GameManager.Players)
                {
                    ImGui.Text($"Player: {id}");
                    var isDrawer = player.IsDrawer;
                    ImGui.Checkbox("Drawer", ref isDrawer);
                    player.IsDrawer = isDrawer;
                    ImGui.Separator();
                }
                ImGui.End();
            });
        }
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}