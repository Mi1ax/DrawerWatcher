﻿using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.GUI;
using CouscousEngine.Utils;
using Drawer_Watcher.Managers;
using ImGuiNET;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace Drawer_Watcher.Screens;

public class LobbyScreen : Screen
{
    private readonly Rectangle _bounds;

    private string _drawerName = "Empty";
    private readonly List<string> _watchersNames = new();

    private readonly Button _joinDrawerButton;
    private readonly Button _joinWatcherButton;

    private readonly Vector2 _topRightPanel;
    private readonly Vector2 _bottomRightPanel;
    
    public LobbyScreen()
    {
        var screenSize = Application.Instance.GetSize();
        _bounds = new Rectangle(
            new Size(screenSize.Width / 2, screenSize.Height - 220), 
            new Vector2(screenSize.Width / 2 - screenSize.Width / 4, 120)
            )
        {
            Color = Color.GRAY
        };
        
        _topRightPanel = new Vector2(_bounds.Position.X + _bounds.Size.Width / 2, _bounds.Position.Y);
        _bottomRightPanel = new Vector2(
            _bounds.Position.X + _bounds.Size.Width / 2, 
            _bounds.Position.Y + _bounds.Size.Height / 2
            );

        _joinDrawerButton = new Button("Join", 
            new Size(145, 45), 
            new Vector2(
                _topRightPanel.X + _bounds.Size.Width / 4 - 145 / 2f,
                _topRightPanel.Y + _bounds.Size.Height / 2 - 45 - 25
            ),
            () =>
            {
                if (Player.ApplicationOwner == null) return;
                if (GameManager.Players.Values.Any(player => player.IsDrawer))
                    return;
                
                Player.ApplicationOwner.IsDrawer = true;
            }
        );
        
        _joinWatcherButton = new Button("Join", 
            new Size(145, 45), 
            new Vector2(
                _bottomRightPanel.X + _bounds.Size.Width / 4 - 145 / 2f,
                _bottomRightPanel.Y + _bounds.Size.Height / 2 - 45 - 25
            ),
            () =>
            {
                if (Player.ApplicationOwner == null) return;

                if (Player.ApplicationOwner.IsDrawer)
                    Player.ApplicationOwner.IsDrawer = false;
            }
        );
    }

    private void DrawDrawerPanel()
    {
        var textSize = _rl.MeasureTextEx(
            AssetManager.GetFont("RobotoMono-Regular"), 
            "Drawer", 24f, 1f
            );
        
        _rl.DrawTextEx(AssetManager.GetFont("RobotoMono-Regular"), "Drawer", 
            new Vector2(_topRightPanel.X + _bounds.Size.Width / 4 - textSize.X / 2, 
            _topRightPanel.Y + 25), 
            24f, 1f, Color.BLACK);

        foreach (var (id, player) in GameManager.Players)
        {
            if (player.IsDrawer)
            {
                _drawerName = id.ToString();
                break;
            }

            _drawerName = "Empty";
        }
        
        var nameSize = _rl.MeasureTextEx(AssetManager.GetFont("RobotoMono-Regular"), _drawerName, 24f, 1f);

        _rl.DrawTextEx(AssetManager.GetFont("RobotoMono-Regular"), _drawerName, 
            new Vector2(_topRightPanel.X + _bounds.Size.Width / 4 - nameSize.X / 2, 
            _topRightPanel.Y + _bounds.Size.Height / 4 - 15), 
            24f, 1f, Color.BLACK);

        _joinDrawerButton.Update();
    }

    private void DrawWatchersPanel()
    {
        var textSize = _rl.MeasureTextEx(AssetManager.GetFont("RobotoMono-Regular"), "Watchers", 24f, 1f);

        _rl.DrawTextEx(AssetManager.GetFont("RobotoMono-Regular"), "Watchers",
            new Vector2(_bottomRightPanel.X + _bounds.Size.Width / 4 - textSize.X / 2,
            _bottomRightPanel.Y + 25),
            24f, 1f, Color.BLACK);
        
        foreach (var (id, player) in GameManager.Players)
        {
            if (player.IsDrawer)
            {
                if (_watchersNames.Contains(id.ToString()))
                    _watchersNames.Remove(id.ToString());
                continue;
            }
            if (!_watchersNames.Contains(id.ToString()))
                _watchersNames.Add(id.ToString());
        }

        if (_watchersNames.Count != 0)
        {
            var maxNameSize = _watchersNames.Max(name => name.Length);
            var longestName = _watchersNames.Find(name => name.Length == maxNameSize);

            if (longestName != null)
            {
                for (var i = 0; i < _watchersNames.Count; i++)
                {
                    var nameSize = _rl.MeasureTextEx(AssetManager.GetFont("RobotoMono-Regular"), 
                        longestName, 24f, 1f);
                    _rl.DrawTextEx(AssetManager.GetFont("RobotoMono-Regular"), _watchersNames[i], 
                        new Vector2(_bottomRightPanel.X + 25 + (nameSize.X + 15) * (i % 3), 
                        // ReSharper disable once PossibleLossOfFraction
                        _bottomRightPanel.Y + 50 + textSize.Y + textSize.Y * (i / 3)), 
                        24f, 1f, Color.BLACK);
                }
            }
        }
        else
        {
            var emptySize = _rl.MeasureTextEx(AssetManager.GetFont("RobotoMono-Regular"), "Empty", 24f, 1f);
            _rl.DrawTextEx(AssetManager.GetFont("RobotoMono-Regular"), "Empty", 
                new Vector2(_bottomRightPanel.X + _bounds.Size.Width / 4 - emptySize.X / 2, 
                _bottomRightPanel.Y + _bounds.Size.Height / 4 - 15), 
                24f, 1f, Color.BLACK);
        }
        
        _joinWatcherButton.Update();
    }
    
    public override void OnUpdate()
    {
        _bounds.Update();
        Renderer.DrawRectangleLines(_bounds, 1f, Color.BLACK);
        
        _rl.DrawLineV(
            _topRightPanel,
            new Vector2(_bounds.Position.X + _bounds.Size.Width / 2, _bounds.Position.Y + _bounds.Size.Height),
            Color.BLACK
            );
        _rl.DrawLineV(
            new Vector2(_bounds.Position.X + _bounds.Size.Width / 2, _bounds.Position.Y + _bounds.Size.Height / 2),
            new Vector2(_bounds.Position.X + _bounds.Size.Width, _bounds.Position.Y + _bounds.Size.Height / 2),
            Color.BLACK
            );
        
        DrawDrawerPanel();
        DrawWatchersPanel();
    }

    public override void OnImGuiUpdate()
    {
        if (!NetworkManager.IsHost) return;

        ImGui.Begin("Lobby");
        if (ImGui.Button("Start Round"))
            GameLogic.StartRound();
        if (ImGui.Button("Stop Round"))
            GameLogic.StopRound();
        ImGui.Text($"Timer: {GameLogic.Timer.Minutes}:{GameLogic.Timer.Seconds}");
        ImGui.Text($"Finished: {GameLogic.FinishedTime.Minutes}:{GameLogic.FinishedTime.Seconds}");
        ImGui.Text($"Word: {GameLogic.CurrentWord}");
        ImGui.Separator();
        foreach (var (id, player) in GameManager.Players)
        {
            ImGui.Text($"Player: {id}, Drawer: {player.IsDrawer}");
            ImGui.Separator();
        }
        ImGui.End();

    }

    public override void Dispose()
    {
        
    }
}