using System.Numerics;
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
        ImGui.Begin("Connection##connect", ref IsVisible, ImGuiWindowFlags.NoDocking |
                                                          ImGuiWindowFlags.NoResize |
                                                          ImGuiWindowFlags.NoMove | 
                                                          ImGuiWindowFlags.NoCollapse);
        {
            ImGui.Text($"Nickname: ({nickname})");
            ImGui.InputText("IP", ref connectionInfo.Ip, 128);
            ImGui.InputInt("Port", ref connectionInfo.Port, 6);
            if (ImGui.Button("Connect"))
            {
                NetworkManager.ConnectToServer(connectionInfo, nickname);
                IsVisible = false;
                LobbyWindow.IsVisible = true;
            }
            
            if (ImGui.Button("Connect locally"))
            {
                NetworkManager.ConnectToServer(ConnectionInfo.Local, nickname);
                IsVisible = false;
                LobbyWindow.IsVisible = true;
            }
            ImGui.End();
        }
    }
}