using System.Numerics;
using CouscousEngine.Networking;
using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class ServerCreationWindow
{
    public static bool IsVisible;

    public static void OnImGuiUpdate(string nickname, ref ConnectionInfo connectionInfo)
    {
        if (!IsVisible) return;
        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
        ImGui.Begin("Connection##create", ref IsVisible, SettingsData.WindowFlags);
        {
            ImGui.Text($"Nickname: ({nickname})");
            ImGui.InputText("IP", ref connectionInfo.Ip, 128);
            ImGui.InputInt("Port", ref connectionInfo.Port, 6);
            if (ImGui.Button("Create Game"))
                Connect(connectionInfo, nickname);

            if (ImGui.Button("Create locally"))
                Connect(ConnectionInfo.Local, nickname);
            ImGui.End();
        }
    }

    private static void Connect(ConnectionInfo info, string nickname)
    {
        NetworkManager.Players.Clear();
        ServerManager.Server.Stop();
        NetworkManager.StartServer(ConnectionInfo.Local);
        NetworkManager.ConnectToServer(ConnectionInfo.Local, nickname);
        IsVisible = false;
        LobbyWindow.IsVisible = true;
    }
}