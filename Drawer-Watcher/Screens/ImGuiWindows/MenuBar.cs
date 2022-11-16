using CouscousEngine.Core;
using Drawer_Watcher.Localization;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class MenuBar
{
    public static void OnImGuiUpdate(Action? additionalMenu = null, string? exitName = null, Action? onExit = null)
    {
        exitName ??= LanguageSystem.GetLocalized("Exit");
        ImGui.BeginMenuBar();
        {
            if (ImGui.BeginMenu(LanguageSystem.GetLocalized("GameName")))
            {
                if (ImGui.MenuItem(LanguageSystem.GetLocalized("Tutorial")))
                    TutorialWindow.IsVisible = true;
                if (ImGui.MenuItem(LanguageSystem.GetLocalized("Settings"), ScreenManager.CurrentScreen is not GameScreen))
                    SettingsWindow.IsVisible = true;
                if (ImGui.MenuItem(exitName))
                {
                    if (onExit == null)
                    {
                        MessageBox.Show(LanguageSystem.GetLocalized("Exit"), 
                            LanguageSystem.GetLocalized("AreYouSure"),
                            MessageBoxButtons.YesNo, button =>
                            {
                                if (button == MessageBoxResult.Yes)
                                    Application.Instance.Close();
                            });
                    } else onExit.Invoke();
                }
                ImGui.EndMenu();
            }
            additionalMenu?.Invoke();
            ImGui.EndMenuBar();
        }
        
        SettingsWindow.OnImGuiUpdate();
        TutorialWindow.OnImGuiUpdate();
    }
}