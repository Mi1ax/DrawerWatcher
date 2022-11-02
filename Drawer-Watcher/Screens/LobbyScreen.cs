using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.GUI;
using CouscousEngine.Utils;
using Drawer_Watcher.Managers;
using Color = CouscousEngine.Utils.Color;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace Drawer_Watcher.Screens;

public class NewLobbyScreen : Screen
{
    private readonly Raylib_CsLo.Rectangle _frame;

    private readonly Raylib_CsLo.Rectangle _settingsFrame;
    private readonly Raylib_CsLo.Rectangle _lobbyFrame;
    
    public NewLobbyScreen()
    {
        // TODO: Temp positions/sizes
        _frame = new Raylib_CsLo.Rectangle(215, 85, 850, 550);

        _settingsFrame = new Raylib_CsLo.Rectangle(242, 110, 390, 500);
        _lobbyFrame = new Raylib_CsLo.Rectangle(649, 110, 390, 500);
    }
    
    public override void OnUpdate()
    {
        _rl.DrawRectangleRounded(_frame, 0.1f, 15, Color.WHITE);
        _rl.DrawRectangleRoundedLines(_settingsFrame, 0.1f, 15, 1f, Color.BLACK);
        _rl.DrawRectangleRoundedLines(_lobbyFrame, 0.1f, 15, 1f, Color.BLACK);
    }

    public override void OnImGuiUpdate()
    {
        
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

public class LobbyScreen : Screen
{
    private readonly Rectangle _bounds;

    private string _drawerName = "Empty";
    private readonly List<string> _watchersNames = new();

    private readonly Button _joinDrawerButton;
    private readonly Button _joinWatcherButton;
    private readonly Button _startGameButton;

    private readonly Vector2 _topRightPanel;
    private readonly Vector2 _bottomRightPanel;
    
    public LobbyScreen()
    {
        var screenSize = Application.Instance.WindowSize;
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

        _startGameButton = new Button(new Raylib_CsLo.Rectangle(
            _bounds.Position.X + _bounds.Size.Width / 4 - 145 / 2f,
            _bounds.Position.Y + _bounds.Size.Height / 2 - 45 - 25,
            145, 45))
        {
            Text = "Start",
            OnButtonClick = (sender, args) =>
            {
                // TODO: Temp
                //if (_drawerName == "Empty" || _watchersNames.Count == 0) return;

                NetworkManager.StartGame();
            }
        };
        
        _joinDrawerButton = new Button(new Raylib_CsLo.Rectangle(
            _topRightPanel.X + _bounds.Size.Width / 4 - 145 / 2f,
            _topRightPanel.Y + _bounds.Size.Height / 2 - 45 - 25,
            145, 45))
        {
            Text = "Join",
            OnButtonClick = (sender, args) =>
            {
                if (Player.ApplicationOwner == null) return;
                if (NetworkManager.Players.Values.Any(player => player.IsDrawer))
                    return;
                
                Player.ApplicationOwner.SetDrawerWithNotifyngServer(true);
            }
        };
        
        _joinWatcherButton = new Button(new Raylib_CsLo.Rectangle(
            _bottomRightPanel.X + _bounds.Size.Width / 4 - 145 / 2f,
            _bottomRightPanel.Y + _bounds.Size.Height / 2 - 45 - 25,
            145, 45))
        {
            Text = "Join",
            OnButtonClick = (sender, args) =>
            {
                if (Player.ApplicationOwner == null) return;

                if (Player.ApplicationOwner.IsDrawer)
                    Player.ApplicationOwner.SetDrawerWithNotifyngServer(false);
            }
        };
    }

    private void DrawSettingsPanel()
    {
        var textSize = _rl.MeasureTextEx(
            AssetManager.GetDefaultFont(24), 
            "Settings", 24f, 1f
        );
        
        _rl.DrawTextEx(AssetManager.GetDefaultFont(24), "Settings", 
            new Vector2(
                _bounds.Position.X + _bounds.Size.Width / 4 - textSize.X / 2,
                _bounds.Position.Y + 25
                ), 24f, 1f, Color.BLACK);
        
        if (NetworkManager.IsHost)
            _startGameButton.OnUpdate();
    }

    private void DrawDrawerPanel()
    {
        var textSize = _rl.MeasureTextEx(
            AssetManager.GetDefaultFont(24), 
            "Drawer", 24f, 1f
            );
        
        _rl.DrawTextEx(AssetManager.GetDefaultFont(24), "Drawer", 
            new Vector2(_topRightPanel.X + _bounds.Size.Width / 4 - textSize.X / 2, 
            _topRightPanel.Y + 25), 
            24f, 1f, Color.BLACK);

        foreach (var (_, player) in NetworkManager.Players)
        {
            if (player.IsDrawer)
            {
                _drawerName = player.Nickname;
                break;
            }

            _drawerName = "Empty";
        }
        
        var nameSize = _rl.MeasureTextEx(AssetManager.GetDefaultFont(24), _drawerName, 24f, 1f);

        _rl.DrawTextEx(AssetManager.GetDefaultFont(24), _drawerName, 
            new Vector2(_topRightPanel.X + _bounds.Size.Width / 4 - nameSize.X / 2, 
            _topRightPanel.Y + _bounds.Size.Height / 4 - 15), 
            24f, 1f, Color.BLACK);

        _joinDrawerButton.OnUpdate();
    }

    private void DrawWatchersPanel()
    {
        var textSize = _rl.MeasureTextEx(AssetManager.GetDefaultFont(24), "Watchers", 24f, 1f);

        _rl.DrawTextEx(AssetManager.GetDefaultFont(24), "Watchers",
            new Vector2(_bottomRightPanel.X + _bounds.Size.Width / 4 - textSize.X / 2,
            _bottomRightPanel.Y + 25),
            24f, 1f, Color.BLACK);
        
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

        if (_watchersNames.Count != 0)
        {
            var maxNameSize = _watchersNames.Max(name => name.Length);
            var longestName = _watchersNames.Find(name => name.Length == maxNameSize);

            if (longestName != null)
            {
                for (var i = 0; i < _watchersNames.Count; i++)
                {
                    var nameSize = _rl.MeasureTextEx(AssetManager.GetDefaultFont(24), 
                        longestName, 24f, 1f);
                    _rl.DrawTextEx(AssetManager.GetDefaultFont(24), _watchersNames[i], 
                        new Vector2(_bottomRightPanel.X + 25 + (nameSize.X + 15) * (i % 3), 
                        // ReSharper disable once PossibleLossOfFraction
                        _bottomRightPanel.Y + 50 + textSize.Y + textSize.Y * (i / 3)), 
                        24f, 1f, Color.BLACK);
                }
            }
        }
        else
        {
            var emptySize = _rl.MeasureTextEx(AssetManager.GetDefaultFont(24), "Empty", 24f, 1f);
            _rl.DrawTextEx(AssetManager.GetDefaultFont(24), "Empty", 
                new Vector2(_bottomRightPanel.X + _bounds.Size.Width / 4 - emptySize.X / 2, 
                _bottomRightPanel.Y + _bounds.Size.Height / 4 - 15), 
                24f, 1f, Color.BLACK);
        }
        
        _joinWatcherButton.OnUpdate();
    }

    private void DrawLines()
    {
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
    }
    
    public override void OnUpdate()
    {
        _bounds.Update();
        Renderer.DrawRectangleLines(_bounds, 1f, Color.BLACK);

        DrawSettingsPanel();
        DrawLines();
        DrawDrawerPanel();
        DrawWatchersPanel();
    }

    public override void OnImGuiUpdate()
    {
        
    }

    public override void Dispose()
    {
        
    }
}