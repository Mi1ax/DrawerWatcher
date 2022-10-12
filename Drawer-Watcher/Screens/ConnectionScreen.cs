using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class ConnectionScreen : Screen
{
    public override void OnUpdate()
    {
        
    }

    public override void OnImGuiUpdate()
    {
        ImGui.Begin("Connection");
        {
            ImGui.InputText("IP", ref ConnectionInfo.Ip, 128);
            ImGui.InputInt("Port", ref ConnectionInfo.Port, 6);
            if (ImGui.Button("Connect"))
            {
                NetworkManager.Connect();
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