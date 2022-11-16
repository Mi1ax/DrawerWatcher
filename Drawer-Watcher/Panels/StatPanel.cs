using CouscousEngine.rlImGui;
using Drawer_Watcher.Localization;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Screens.ImGuiWindows;
using ImGuiNET;

namespace Drawer_Watcher.Panels;

public static class StatPanel
{
    public static void OnImGuiUpdate(ImGuiWindowFlags flags = ImGuiWindowFlags.None)
    {
        ImGui.Begin("Stats", flags);
        ImGui.Text($"{LanguageSystem.GetLocalized("Players")}:");
        if (SettingsData.IsHintsOn)
        {
            ImGui.SameLine();
            rlImGui.HelpMarker(LanguageSystem.GetLocalized("StatsHint"));
        }
        ImGui.Separator();
        foreach (var player in NetworkManager.Players.Values)
        {
            if (player.IsInLobby) continue;
            var text = $"{player.Nickname} ";
            if (!player.IsDrawer)
                text += $"{LanguageSystem.GetLocalized("Score")}: {player.Score} ";
            else
                text += $"<- {LanguageSystem.GetLocalized("Drawer")}";
            ImGui.Text(text);
        }
        ImGui.End();
    }
}