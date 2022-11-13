using CouscousEngine.Core;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class MenuBar
{
    public static void OnImGuiUpdate(Action? additionalMenu = null, Action? onExit = null)
    {
        ImGui.BeginMenuBar();
        {
            if (ImGui.BeginMenu("Drawer Watcher"))
            {
                if (ImGui.MenuItem("Settings", ScreenManager.CurrentScreen is not GameScreen))
                    SettingsWindow.IsVisible = true;
                if (ImGui.MenuItem("Exit"))
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