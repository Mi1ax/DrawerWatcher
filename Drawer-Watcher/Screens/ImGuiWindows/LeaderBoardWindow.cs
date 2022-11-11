using System.Numerics;
using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class LeaderBoardWindow
{
    public static bool IsVisible;

    public static void OnImGuiUpdate()
    {
        if (!IsVisible) return;
        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
        ImGui.Begin("Leaders:", SettingsData.WindowFlags);
        {
            var sortedDict = (
                    from entry in NetworkManager.Players.Values 
                    where !entry.IsDrawer 
                    orderby entry.Score
                    select entry).ToList();
            for (var i = 0; i < sortedDict.Count; i++)
            {
                ImGui.Text($"{i + 1}. {sortedDict[i].Nickname}: {sortedDict[i].Score}");
                ImGui.Separator();
            }

            if (Player.ApplicationOwner!.IsDrawer)
            {
                if (ImGui.SmallButton("New round"))
                    GameScreen.NewRound();
            }
            if (ImGui.SmallButton("Exit"))
            {
                GameManager.IsGameStarted = false;
                MessageHandlers.SendLobbyExit();
                IsVisible = false;
                ScreenManager.NavigateTo(new MenuScreen());
            }
            ImGui.End();
        }
    }
}