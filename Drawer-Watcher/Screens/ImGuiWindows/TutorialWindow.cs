using Drawer_Watcher.Localization;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class TutorialWindow
{
    public static bool IsVisible;

    public static void OnImGuiUpdate()
    {
        if (!IsVisible) return;
        ImGui.Begin(LanguageSystem.GetLocalized("Tutorial"), ref IsVisible, ImGuiWindowFlags.NoDocking);
        {
            
            ImGui.End();
        }
    }
}