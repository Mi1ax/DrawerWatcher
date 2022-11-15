using System.Numerics;
using CouscousEngine.Networking;
using Drawer_Watcher.Localization;
using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class ConnectionWindow
{
    public static bool IsVisible;

    public static void OnImGuiUpdate(string nickname, ref ConnectionInfo connectionInfo)
    {
        if (!IsVisible) return;
        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
        ImGui.Begin($"{LanguageSystem.GetLocalized("Connection")}##connect", ref IsVisible, SettingsData.WindowFlags);
        {
            ImGui.Text($"{LanguageSystem.GetLocalized("Nickname")}: ({nickname})");
            ImGui.InputText("IP", ref connectionInfo.Ip, 128);
            ImGui.InputInt("Port", ref connectionInfo.Port, 6);
            if (ImGui.Button(LanguageSystem.GetLocalized("Connect")))
                Connect(connectionInfo, nickname);
            if (ImGui.Button(LanguageSystem.GetLocalized("ConnectLocally")))
                Connect(ConnectionInfo.Local, nickname);
            ImGui.End();
        }
    }
    
    private static void Connect(ConnectionInfo info, string nickname)
    {
        LobbyWindow.Clear();
        NetworkManager.Players.Clear();
        ServerManager.Server.Stop();
        NetworkManager.ConnectToServer(info, nickname);
        IsVisible = false;
        LobbyWindow.IsVisible = true;
    }
}