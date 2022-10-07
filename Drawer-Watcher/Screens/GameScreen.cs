using System.Numerics;
using CouscousEngine.rlImGui;
using CouscousEngine.Shapes;
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
        
        if (NetworkManager.IsHost)
        {
            rlImGui.OnUpdate(() =>
            {
                ImGui.Begin("Players");
                foreach (var (id, player) in GameManager.Players)
                {
                    ImGui.Text($"Player {player.IsApplicationOwner}: {id}");
                    var isDrawer = player.IsDrawer;
                    ImGui.Checkbox($"Drawer##{id}", ref isDrawer);
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