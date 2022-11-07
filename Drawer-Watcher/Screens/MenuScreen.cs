using System.Numerics;
using CouscousEngine.Core;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Screens.ImGuiWindows;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class MenuScreen : Screen
{
    private string _nickname = "Player";
    private ConnectionInfo _connectionInfo = ConnectionInfo.Default;
    private bool _showMenu = true;

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
                ImGui.DockSpace(dockspaceID, Vector2.Zero, ImGuiDockNodeFlags.NoResize 
                                                           | ImGuiDockNodeFlags.AutoHideTabBar);
            }
            
            ImGui.BeginMenuBar();
            {
                // TODO: MessageBox
                if (ImGui.BeginMenu("Drawer Watcher"))
                {
                    if (ImGui.MenuItem("Exit"))
                    {
                        MessageBox.Show("Exit", "Are you sure?", 
                            MessageBoxButtons.YesNo, button =>
                            {
                                if (button == MessageBoxResult.Yes)
                                    Application.Instance.Close();
                            });
                    }

                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            if (_showMenu)
            {
                ImGui.Begin("Menu");
                {
                    ImGui.Text("Nickname:");
                    ImGui.InputText("", ref _nickname, 32);
                    if (ImGui.Button("Host"))
                    {
                        _showMenu = false;
                        ServerCreationWindow.IsVisible = true;
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Connect"))
                    {
                        _showMenu = false;
                        ConnectionWindow.IsVisible = true;
                    }

                    ImGui.End();
                }
            }
            
            MessageBox.OnImGuiUpdate();
            LobbyWindow.OnImGuiUpdate();
            ServerCreationWindow.OnImGuiUpdate(_nickname, ref _connectionInfo);
            ConnectionWindow.OnImGuiUpdate(_nickname, ref _connectionInfo);
            
            ImGui.End();
        }
    }
}