using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Panels;

public static class StatPanel
{
    public static void OnImGuiUpdate(ImGuiWindowFlags flags = ImGuiWindowFlags.None)
    {
        ImGui.Begin("Stats", flags);
        ImGui.Text("Players:");
        ImGui.Separator();
        foreach (var player in NetworkManager.Players.Values)
        {
            if (player.IsInLobby) continue;
            var text = $"{player.Nickname} ";
            if (!player.IsDrawer)
                text += $"Score: {player.Score} ";
            else
                text += "<- Drawer";
            ImGui.Text(text);
        }
        ImGui.End();
    }
}