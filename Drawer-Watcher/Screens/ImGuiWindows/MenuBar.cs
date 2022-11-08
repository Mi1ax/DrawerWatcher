using CouscousEngine.Core;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class MenuBar
{
    public static void OnImGuiUpdate()
    {
        ImGui.BeginMenuBar();
        {
            if (ImGui.BeginMenu("Drawer Watcher"))
            {
                if (ImGui.MenuItem("Settings"))
                    SettingsWindow.IsVisible = true;
                if (ImGui.MenuItem("Exit"))
                {
                    MessageBox.Show("Exit", "Are you sure?", 
                        MessageBoxButtons.YesNo, button =>
                        {
                            if (button == MessageBoxResult.Yes)
                                Application.Instance.Close();
                        });
                }
                ImGui.EndMenu();
            }
            ImGui.EndMenuBar();
        }
        
        SettingsWindow.OnImGuiUpdate();
    }
}