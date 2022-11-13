using System.ComponentModel;
using CouscousEngine.Utils;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public enum Resolutions
{
    [Description("1280x720")]
    _1280x720,
    [Description("960x480")]
    _960x480
}

public static class SettingsData
{
    private static Resolutions _resolution =
        Resolutions._1280x720;
    public static Resolutions Resolution
    {
        get => _resolution;
        set
        {
            var windowWidth = Convert.ToInt32(value.GetDescription()?.Split('x')[0]);
            var windowHeight = Convert.ToInt32(value.GetDescription()?.Split('x')[1]);
            _rl.SetWindowSize(windowWidth, windowHeight);
            _resolution = value;
        }
    }

    public const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoDocking 
                                                | ImGuiWindowFlags.AlwaysAutoResize 
                                                | ImGuiWindowFlags.NoResize 
                                                | ImGuiWindowFlags.NoMove 
                                                | ImGuiWindowFlags.NoCollapse;
}

public static class SettingsWindow
{
    public static bool IsVisible;
    
    public static void OnImGuiUpdate()
    {
        if (!IsVisible || ScreenManager.CurrentScreen is GameScreen) return;
        ImGui.Begin("Settings", ref IsVisible);
        {
            ImGui.Text($"Resolution (Current {SettingsData.Resolution.GetDescription()}):");
            if (ImGui.Button("1280x720"))
                SettingsData.Resolution = Resolutions._1280x720;
            if (ImGui.Button("960x480"))
                SettingsData.Resolution = Resolutions._960x480;
            ImGui.End();
        }
    }
}