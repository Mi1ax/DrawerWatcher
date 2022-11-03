using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class CreatingGameScreen : Screen
{
    private ConnectionInfo _connectionInfo = ConnectionInfo.Default;
    private string _nickname;

    public CreatingGameScreen(string nickname)
    {
        _nickname = nickname;
    }
    
    public override void OnUpdate(float deltaTime)
    {
        
    }

    public override void OnImGuiUpdate()
    {
        ImGui.Begin("Connection");
        {
            ImGui.InputText("Nickname", ref _nickname, 32);
            ImGui.InputText("IP", ref _connectionInfo.Ip, 128);
            ImGui.InputInt("Port", ref _connectionInfo.Port, 6);
            if (ImGui.Button("Create Game"))
            {
                NetworkManager.StartServer(_connectionInfo);
                NetworkManager.ConnectToServer(_connectionInfo, _nickname);
                ScreenManager.NavigateTo(new LobbyScreen());
            }
            
            if (ImGui.Button("Create locally"))
            {
                NetworkManager.StartServer(ConnectionInfo.Local);
                NetworkManager.ConnectToServer(ConnectionInfo.Local, _nickname);
                ScreenManager.NavigateTo(new LobbyScreen());
            }
        }
        ImGui.End();
    }

    public override bool OnEvent()
    {
        return false;
    }
}