using System.Numerics;
using CouscousEngine.Core;
using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class MenuScreen : Screen
{
    private string _nickname = "Player";

    private ConnectionInfo _connectionInfo = ConnectionInfo.Default;

    private bool _showServerCreation;
    private bool _showServerConnection;

    public override void OnImGuiUpdate()
    {
        var window_flags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
        
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        ImGui.SetNextWindowViewport(viewport.ID);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        window_flags |= ImGuiWindowFlags.NoTitleBar | 
                        ImGuiWindowFlags.NoCollapse | 
                        ImGuiWindowFlags.NoResize | 
                        ImGuiWindowFlags.NoMove;
        window_flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | 
                        ImGuiWindowFlags.NoNavFocus;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

        ImGui.Begin("MainDockspace", window_flags);
        {
            ImGui.PopStyleVar();
            
            ImGui.PopStyleVar(2);

            var io = ImGui.GetIO();

            if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.DockingEnable))
            {
                var dockspaceID = ImGui.GetID("MyDockSpace");
                ImGui.DockSpace(dockspaceID, Vector2.Zero, ImGuiDockNodeFlags.NoResize);
            }

            ImGui.BeginMenuBar();
            {
                // TODO: MessageBox
                if (ImGui.BeginMenu("Drawer Watcher"))
                {
                    if (ImGui.MenuItem("Exit"))
                        Application.Instance.Close();
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }
            
            ImGui.Begin("Menu");
            {
                ImGui.Text("Nickname:");
                ImGui.InputText("", ref _nickname, 32);
                if (ImGui.Button("Host"))
                    _showServerCreation = true;
            
                ImGui.SameLine();
                if (ImGui.Button("Connect"))
                    _showServerConnection = true;
            
                ImGui.End();
            }

            if (_showServerCreation)
            {
                ImGui.Begin("Connection##create", ref _showServerCreation, ImGuiWindowFlags.NoDocking);
                {
                    ImGui.Text($"Nickname {_nickname}");
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
                    ImGui.End();
                }
            }

            if (_showServerConnection)
            {
                ImGui.Begin("Connection##connect", ref _showServerConnection, ImGuiWindowFlags.NoDocking);
                {
                    ImGui.Text($"Nickname {_nickname}");
                    ImGui.InputText("IP", ref _connectionInfo.Ip, 128);
                    ImGui.InputInt("Port", ref _connectionInfo.Port, 6);
                    if (ImGui.Button("Connect"))
                    {
                        NetworkManager.ConnectToServer(_connectionInfo, _nickname);
                        ScreenManager.NavigateTo(new LobbyScreen());
                    }
            
                    if (ImGui.Button("Connect locally"))
                    {
                        NetworkManager.ConnectToServer(ConnectionInfo.Local, _nickname);
                        ScreenManager.NavigateTo(new LobbyScreen());
                    }
                    ImGui.End();
                }
            }
            
            ImGui.End();
        }
    }
}