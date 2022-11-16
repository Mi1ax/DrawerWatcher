using System.Numerics;
using CouscousEngine.Networking;
using CouscousEngine.rlImGui;
using Drawer_Watcher.Localization;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Screens.ImGuiWindows;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class MenuScreen : Screen
{
    private string _nickname = LanguageSystem.GetLocalized("Player");
    private ConnectionInfo _connectionInfo = ConnectionInfo.Default;

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
            
            MenuBar.OnImGuiUpdate();

            if (!ServerCreationWindow.IsVisible && !ConnectionWindow.IsVisible && !LobbyWindow.IsVisible)
            {
                if (ClientManager.Client!.IsConnected)
                    ClientManager.Client.Disconnect();
                if (ServerManager.Server.IsRunning)
                    ServerManager.Server.Stop();

                var center = ImGui.GetMainViewport().GetCenter();
                ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
                ImGui.Begin(LanguageSystem.GetLocalized("Menu"), SettingsData.WindowFlags);
                {
                    ImGui.Text($"{LanguageSystem.GetLocalized("Nickname")}:");
                    if (SettingsData.IsHintsOn)
                    {
                        ImGui.SameLine();
                        rlImGui.HelpMarker(LanguageSystem.GetLocalized("NicknameHint"));
                    }
                    ImGui.InputText("", ref _nickname, 32);
                    if (ImGui.Button(LanguageSystem.GetLocalized("Host")))
                        ServerCreationWindow.IsVisible = true;
                    if (SettingsData.IsHintsOn)
                    {
                        ImGui.SameLine();
                        rlImGui.HelpMarker(LanguageSystem.GetLocalized("HostHint"));
                    }

                    ImGui.SameLine();
                    if (ImGui.Button(LanguageSystem.GetLocalized("Connect")))
                        ConnectionWindow.IsVisible = true;
                    if (SettingsData.IsHintsOn)
                    {
                        ImGui.SameLine();
                        rlImGui.HelpMarker(LanguageSystem.GetLocalized("ConnectHint"));
                    }

                    ImGui.End();
                }
            }
            
            LobbyWindow.OnImGuiUpdate();
            ServerCreationWindow.OnImGuiUpdate(_nickname, ref _connectionInfo);
            ConnectionWindow.OnImGuiUpdate(_nickname, ref _connectionInfo);
            
            ImGui.End();
        }
    }
}