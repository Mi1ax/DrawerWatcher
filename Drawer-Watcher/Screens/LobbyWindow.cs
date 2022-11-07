using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class LobbyWindow
{
    private readonly List<string> _watchersNames = new ();
    
    public void OnImGuiUpdate()
    {
        ImGui.Begin("Lobby", ImGuiWindowFlags.NoDocking);
        {
            if (ImGui.BeginTable("table", 2, ImGuiTableFlags.BordersInnerV))
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text("Game Settings");
                if (NetworkManager.IsHost)
                {
                    if (ImGui.Button("Start"))
                    {
                        NetworkManager.StartGame();
                    }
                }
                
                ImGui.TableNextColumn();
                ImGui.Text("Drawers");
                var drawerName = "Empty";
                foreach (var (_, player) in NetworkManager.Players)
                {
                    if (player.IsDrawer)
                    {
                        drawerName = player.Nickname;
                        break;
                    }
                }
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
                ImGui.Text("Watchers");
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