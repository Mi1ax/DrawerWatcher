using System.Numerics;
using CouscousEngine.Networking;
using Drawer_Watcher.Localization;
using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public static class LobbyWindow
{
    public static bool IsVisible;
    
    public static readonly List<string> WatchersNames = new();

    private static int _minutes = 1;

    public static void Clear() => WatchersNames.Clear();
    
    public static void OnImGuiUpdate()
    {
        if (!IsVisible) return;

        if (ClientManager.Client!.IsConnecting)
        {
            var center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            ImGui.Begin(LanguageSystem.GetLocalized("Connection"), ImGuiWindowFlags.NoDocking |
                                                                   ImGuiWindowFlags.NoResize |
                                                                   ImGuiWindowFlags.NoMove | 
                                                                   ImGuiWindowFlags.NoCollapse);
            {
                ImGui.Text(LanguageSystem.GetLocalized("ConnectingToTheServer"));
                ImGui.End();
            }
        } 
        else if (ClientManager.Client.IsNotConnected)
        {
            if (!MessageBox.IsOpen())
                MessageBox.Show(LanguageSystem.GetLocalized("Error"), 
                    $"{LanguageSystem.GetLocalized("ErrorConnectionToServer")}: " + 
                    ClientManager.Client.DisconnectedReason, 
                    MessageBoxButtons.Ok,
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
            ImGui.Begin(LanguageSystem.GetLocalized("Lobby"), ref IsVisible, SettingsData.WindowFlags);
            {
                if (ImGui.BeginTable("table", 2, ImGuiTableFlags.BordersInnerV))
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(LanguageSystem.GetLocalized("GameSettings"));
                    switch (NetworkManager.IsHost)
                    {
                        case true:
                        {
                            ImGui.Text(LanguageSystem.GetLocalized("MinutesForGuessing"));
                            if (ImGui.InputInt("", ref _minutes, 1, 1))
                            {
                                if (_minutes < 1) _minutes = 1;
                                if (_minutes > 15) _minutes = 15;
                            }

                            if (ImGui.Button(LanguageSystem.GetLocalized("Start")))
                            {
                                if (NetworkManager.Players.Values.FirstOrDefault(p => p.IsDrawer) != null)
                                    NetworkManager.StartGame(_minutes);
                            }

                            break;
                        }
                        case false when GameManager.IsGameStarted:
                            ImGui.Text(LanguageSystem.GetLocalized("GameIsRunning"));
                            break;
                    }

                    ImGui.TableNextColumn();
                    var drawerName = LanguageSystem.GetLocalized("Empty");
                    foreach (var (_, player) in NetworkManager.Players)
                    {
                        if (player.IsDrawer && player.Nickname != "DefaultNickname")
                        {
                            drawerName = player.Nickname;
                            break;
                        }
                    }

                    var count = drawerName == "Empty" ? 0 : 1;
                    ImGui.Text($"{LanguageSystem.GetLocalized("Drawers")} ({count})");
                    ImGui.Text(drawerName);
                    if (ImGui.Button($"{LanguageSystem.GetLocalized("Join")}##drawers"))
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
                    ImGui.Text($"{LanguageSystem.GetLocalized("Watchers")} ({WatchersNames.Count})");
                    foreach (var (_, player) in NetworkManager.Players)
                    {
                        if (player.Nickname == "DefaultNickname") continue;
                        if (player.IsDrawer)
                        {
                            if (WatchersNames.Contains(player.Nickname))
                                WatchersNames.Remove(player.Nickname);
                            continue;
                        }
                        if (!WatchersNames.Contains(player.Nickname))
                            WatchersNames.Add(player.Nickname);
                    }
                    
                    if (WatchersNames.Count == 0)
                        ImGui.Text(LanguageSystem.GetLocalized("Empty"));
                    else
                    {
                        foreach (var name in WatchersNames)
                            ImGui.Text(name);
                    }
                    
                    if (ImGui.Button($"{LanguageSystem.GetLocalized("Join")}##watchers"))
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