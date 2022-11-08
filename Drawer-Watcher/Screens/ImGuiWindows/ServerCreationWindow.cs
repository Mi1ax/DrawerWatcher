using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class ServerCreationWindow
{
    public static bool IsVisible;

    public static void OnImGuiUpdate(string nickname, ref ConnectionInfo connectionInfo)
    {
        if (!IsVisible) return;
        ImGui.Begin("Connection##create", ImGuiWindowFlags.NoDocking);
        {
            ImGui.Text($"Nickname {nickname}");
            ImGui.InputText("IP", ref connectionInfo.Ip, 128);
            ImGui.InputInt("Port", ref connectionInfo.Port, 6);
            if (ImGui.Button("Create Game"))
            {
                NetworkManager.StartServer(connectionInfo);
                NetworkManager.ConnectToServer(connectionInfo, nickname);
                IsVisible = false;
                LobbyWindow.IsVisible = true;
            }
            
            if (ImGui.Button("Create locally"))
            {
                NetworkManager.StartServer(ConnectionInfo.Local);
                NetworkManager.ConnectToServer(ConnectionInfo.Local, nickname);
                IsVisible = false;
                LobbyWindow.IsVisible = true;
            }
            ImGui.End();
        }
    }
}