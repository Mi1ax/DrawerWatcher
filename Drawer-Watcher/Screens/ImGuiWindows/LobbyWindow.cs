using System.Numerics;
using CouscousEngine.Networking;
using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class LobbyWindow
{
    public static bool IsVisible;
    
    private static readonly List<string> _watchersNames = new ();

    private static int _minutes = 1;
    
    public static void OnImGuiUpdate()
    {
        if (!IsVisible) return;

        if (ClientManager.Client!.IsConnecting)
        {
            var center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.Begin("Connection", ImGuiWindowFlags.NoDocking |
                                      ImGuiWindowFlags.NoResize |
                                      ImGuiWindowFlags.NoMove | 
                                      ImGuiWindowFlags.NoCollapse);
            {
                ImGui.Text("Connecting to the server ... ");
                ImGui.End();
            }
        } 
        else if (ClientManager.Client.IsNotConnected)
        {
            MessageBox.Show("Error", "An error occurred connecting to the server", MessageBoxButtons.Ok,
                result =>
                {
                    if (result == MessageBoxResult.Ok)
                        IsVisible = false;
                });
        }
        else if (ClientManager.Client.IsConnected)
        {
            var center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.Begin("Lobby", SettingsData.WindowFlags);
            {
                if (ImGui.BeginTable("table", 2, ImGuiTableFlags.BordersInnerV))
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text("Game Settings");
                    switch (NetworkManager.IsHost)
                    {
                        case true:
                        {
                            ImGui.Text("Minutes for guessing");
                            if (ImGui.InputInt("", ref _minutes, 1, 1))
                                if (_minutes < 1) _minutes = 1;
                        
                            if (ImGui.Button("Start"))
                                NetworkManager.StartGame(_minutes);

                            break;
                        }
                        case false when GameManager.IsGameStarted:
                            ImGui.Text("Game is already started. \nWhait until the end");
                            break;
                    }

                    ImGui.TableNextColumn();
                    var drawerName = "Empty";
                    foreach (var (_, player) in NetworkManager.Players)
                    {
                        if (player.IsDrawer)
                        {
                            drawerName = player.Nickname;
                            break;
                        }
                    }

                    var count = drawerName == "Empty" ? 0 : 1;
                    ImGui.Text($"Drawers ({count})");
                    ImGui.Text(drawerName);
                    if (ImGui.Button("Join##drawers"))
                    {
                        if (Player.ApplicationOwner == null) return;
                        if (NetworkManager.Players.Values.Any(player => player.IsDrawer))
                            return;
                    
                        Player.ApplicationOwner.SetDrawerWithNotifyngServer(true);
                    }
                    ImGui.Separator();
                    
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableNextColumn();
                    ImGui.Text($"Watchers ({_watchersNames.Count})");
                    foreach (var (_, player) in NetworkManager.Players)
                    {
                        if (player.IsDrawer)
                        {
                            if (_watchersNames.Contains(player.Nickname))
                                _watchersNames.Remove(player.Nickname);
                            continue;
                        }
                        if (!_watchersNames.Contains(player.Nickname))
                            _watchersNames.Add(player.Nickname);
                    }
                    
                    if (_watchersNames.Count == 0)
                        ImGui.Text("Empty");
                    else
                    {
                        foreach (var name in _watchersNames)
                            ImGui.Text(name);
                    }
                    
                    if (ImGui.Button("Join##watchers"))
                    {
                        if (Player.ApplicationOwner == null) return;

                        if (Player.ApplicationOwner.IsDrawer)
                            Player.ApplicationOwner.SetDrawerWithNotifyngServer(false);
                    }

                    ImGui.EndTable();
                }
                ImGui.End();
            }
        }
    }
}