using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class ConnectionScreen : Screen
{
    private ConnectionInfo _connectionInfo = ConnectionInfo.Default;
    private string _nickname = "Player";
    
    public override void OnUpdate()
    {
        
    }

    public override void OnImGuiUpdate()
    {
        ImGui.Begin("Connection");
        {
            ImGui.InputText("Nickname", ref _nickname, 32);
            ImGui.InputText("IP", ref _connectionInfo.Ip, 128);
            ImGui.InputInt("Port", ref _connectionInfo.Port, 6);
            if (ImGui.Button("Connect"))
            {
                NetworkManager.ConnectToServer(_connectionInfo, _nickname);
                ScreenManager.NavigateTo(new LobbyScreen());
            }

            if (ImGui.Button("Connect local"))
            {
                NetworkManager.ConnectToServer(ConnectionInfo.Local, _nickname);
                ScreenManager.NavigateTo(new LobbyScreen());
            }
        }
        ImGui.End();
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}