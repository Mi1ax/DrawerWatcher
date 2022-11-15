using CouscousEngine.Core;
using Drawer_Watcher.Localization;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class MenuBar
{
    public static void OnImGuiUpdate(Action? additionalMenu = null, string exitName = "Exit", Action? onExit = null)
    {
        ImGui.BeginMenuBar();
        {
            if (ImGui.BeginMenu(LanguageSystem.GetLocalized("GameName")))
            {
                if (ImGui.MenuItem("Settings", ScreenManager.CurrentScreen is not GameScreen))
                    SettingsWindow.IsVisible = true;
                if (ImGui.MenuItem(exitName))
                {
                    if (onExit == null)
                    {
                        MessageBox.Show("Exit", "Are you sure?",
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
    }
}