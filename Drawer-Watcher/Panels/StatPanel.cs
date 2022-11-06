using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Panels;

public class StatPanel
{
    public void OnImGuiUpdate()
    {
        ImGui.Begin("Stats");
        ImGui.Text("Players:");
        ImGui.Separator();
        foreach (var player in NetworkManager.Players.Values)
        {
            ImGui.Text($"{player.Nickname} [{player.ID}]");
        }
        ImGui.End();
    }
}