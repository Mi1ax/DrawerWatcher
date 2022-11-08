using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Panels;

public class StatPanel
{
    public void OnImGuiUpdate(ImGuiWindowFlags flags = ImGuiWindowFlags.None)
    {
        ImGui.Begin("Stats", flags);
        ImGui.Text("Players:");
        ImGui.Separator();
        foreach (var player in NetworkManager.Players.Values)
        {
            var text = $"{player.Nickname} [{player.ID}] ";
            if (!player.IsDrawer)
                text += $"Score: {player.Score}";
            else
                text += "<- Drawer";
            ImGui.Text(text);
        }
        ImGui.End();
    }
}