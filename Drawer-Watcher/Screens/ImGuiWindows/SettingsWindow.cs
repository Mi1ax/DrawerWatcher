using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CouscousEngine.Core;
using CouscousEngine.rlImGui;
using CouscousEngine.Utils;
using Drawer_Watcher.Localization;
using Drawer_Watcher.Managers;
using ImGuiNET;
using IniParser;
using IniParser.Model;

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

    private static bool _isHintsOn;
    
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

    public static bool IsHintsOn
    {
        get => _isHintsOn;
        set
        {
            SettingsIni.AddData("hints", value ? "true" : "false");
            _isHintsOn = value;
        }
    }
    
    public const ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoDocking 
                                                | ImGuiWindowFlags.AlwaysAutoResize 
                                                | ImGuiWindowFlags.NoResize 
                                                | ImGuiWindowFlags.NoMove 
                                                | ImGuiWindowFlags.NoCollapse;
}

public static class SettingsIni
{
    private static string _filePath = null!;
    private static IniData _data = null!;
    
    private static readonly FileIniDataParser _parser = new ();
    
    public static readonly Dictionary<string, string> Default = new();

    public static void Init(string fileName)
    {
        _filePath = fileName;
        _data = _parser.ReadFile(fileName);

        Default["resolution"] = Resolutions._1280x720.GetDescription()!;
        Default["language"] = LanguageSystem.CurrentLanguage.Name;
        Default["hints"] = SettingsData.IsHintsOn ? "true" : "false";
    }

    public static void Load()
    {
        var resolution = GetData("resolution");
        var windowWidth = Convert.ToInt32(resolution.Split('x')[0]);
        var windowHeight = Convert.ToInt32(resolution.Split('x')[1]);
        _rl.SetWindowSize(windowWidth, windowHeight);

        var language = GetData("language");
        var languages = LanguageSystem.Languages.Keys.ToArray();
        for (var i = 0; i < languages.Length; i++)
        {
            if (languages[i] == language)
            {
                LanguageSystem.SelectedLanguageIndex = i;
                break;
            }
        }
        LanguageSystem.SelectLanguage(language);

        SettingsData.IsHintsOn = GetData("hints") == "true";
    }
    
    public static void AddData(string name, string data)
    {
        _data["Settings"][name] = data;
        _parser.WriteFile(_filePath, _data);
    }

    public static string GetData(string name) 
        => _data["Settings"][name] ?? Default[name];
}

public static class SettingsWindow
{
    public static bool IsVisible;
    
    public static void OnImGuiUpdate()
    {
        if (!IsVisible || ScreenManager.CurrentScreen is GameScreen) return;
        var flags = SettingsData.WindowFlags;
        flags -= ImGuiWindowFlags.NoMove;
        ImGui.Begin(LanguageSystem.GetLocalized("Settings"), ref IsVisible, flags);
        {
            ImGui.Text($"{LanguageSystem.GetLocalized("Hints")}: ");
            ImGui.SameLine();
            var value = SettingsData.IsHintsOn;
            if (ImGui.Checkbox("##hints", ref value))
            {
                if (!value.Equals(SettingsData.IsHintsOn))
                    SettingsData.IsHintsOn = value;
            }
                
            ImGui.Text($"{LanguageSystem.GetLocalized("Resolution")} ({LanguageSystem.GetLocalized("CurrentResolution")} {SettingsIni.GetData("resolution")}):");
            if (ImGui.Button("1280x720"))
            {
                SettingsData.Resolution = Resolutions._1280x720;
                SettingsIni.AddData("resolution", "1280x720");
            }

            if (ImGui.Button("960x480"))
            {
                SettingsData.Resolution = Resolutions._960x480;
                SettingsIni.AddData("resolution", "960x480");
            }
            
            ImGui.Text($"{LanguageSystem.GetLocalized("MaxPlayers")}: " +
                       $"({LanguageSystem.GetLocalized("CurrentResolution")} {ConnectionInfo.MaxConnection}):");
            ImGui.SameLine();
            rlImGui.HelpMarker(LanguageSystem.GetLocalized("NeedServerRestart"));
            if (ImGui.InputInt("", ref ConnectionInfo.MaxConnection, 1, 1))
            {
                ConnectionInfo.MaxConnection = ConnectionInfo.MaxConnection switch
                {
                    < 1 => 1,
                    > 6 => 6,
                    _ => ConnectionInfo.MaxConnection
                }; 
            }

            ImGui.Text($"{LanguageSystem.GetLocalized("Language")}: " +
                       $"({LanguageSystem.GetLocalized("CurrentLanguage")} {LanguageSystem.CurrentLanguage.Name}):");
            var languages = LanguageSystem.Languages.Keys.ToArray();
            if (ImGui.Combo("##languages", 
                    ref LanguageSystem.SelectedLanguageIndex,
                    languages, languages.Length))
            {
                var lang = languages[LanguageSystem.SelectedLanguageIndex];
                SettingsIni.AddData("language", lang);
                LanguageSystem.SelectLanguage(lang);
            }

            if (ImGui.SmallButton($"{LanguageSystem.GetLocalized("OpenWords")}"))
            {
                Process.Start(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? new ProcessStartInfo {FileName = @"Assets/Words.txt", UseShellExecute = true}
                    : new ProcessStartInfo {FileName = @"Assets\Words.txt", UseShellExecute = true});
            }
            ImGui.End();
        }
    }
}