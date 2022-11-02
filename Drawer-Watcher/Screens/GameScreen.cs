using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.GUI;
using CouscousEngine.Shapes;
using CouscousEngine.Utils;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Panels;

namespace Drawer_Watcher.Screens;

public class GameScreen : Screen
{
    private readonly Rectangle _drawingPanel;
    private readonly ChatPanel _chatPanel;
    private readonly ToolPanel _toolPanel;

    private readonly Button _button;

    public GameScreen()
    {
        _chatPanel = new ChatPanel(new Rectangle(new Size(350, 720), new Vector2(930, 0))
        {
            Color = new Color(193, 193, 193)
        });
        
        _drawingPanel = new Rectangle(new Size(930, 720 - 144), Vector2.Zero);

        _toolPanel = new ToolPanel(new Rectangle(new Size(930, 144), new Vector2(0, 720 - 144))
        {
            Color = Color.GRAY
        });
        
        GameData.Painting = Renderer.LoadRenderTexture((int)_drawingPanel.Size.Width, (int)_drawingPanel.Size.Height);
        if (Player.ApplicationOwner is {IsDrawer: true})
            _chatPanel.DisableInput = true;

        if (Player.ApplicationOwner is {IsDrawer: true})
            NewWord();

        _button = new Button(new Raylib_CsLo.Rectangle(
            _drawingPanel.Position.X + _drawingPanel.Size.Width / 2 - 125 / 2f,
            _drawingPanel.Position.Y + _drawingPanel.Size.Height / 2 - 55 / 2f,
            125, 55))
        {
            Text = "New Word",
            OnButtonClick = (sender, args) =>
            {
                NewWord();
            }
        };
    }

    public static void InitTimer()
    {
        GameManager.Timer.Init();
        GameManager.Timer.Start(new TimeSpan(0, 0, 10), () =>
        {
            
        });
    }
    
    private static void NewWord()
    {
        foreach (var player in NetworkManager.Players.Values)
            player.Reset();
        MessageHandlers.GetNewWord();
        MessageHandlers.ClearPainting();
        GameManager.Guesser = 0;
    }

    public override void OnUpdate()
    {
        if (!NetworkManager.IsClientConnected || NetworkManager.Players.Count == 0) return;

        Renderer.DrawRectangleLines(_drawingPanel, 1f, Color.BLACK);

        if (GameManager.Timer.Enable)
        {
            if (GameManager.Guesser == 0)
            {
                foreach (var player in NetworkManager.Players.Values)
                    player.Update();
                    
                Renderer.DrawTexture(GameData.Painting!.Value, _drawingPanel.Position, Color.WHITE);

                var text = new Text
                {
                    Font = AssetManager.GetDefaultFont(48),
                    FontSize = 48f,
                    Value = GameManager.Timer.CurrentTime,
                    FontColor = Color.BLACK
                };
                Renderer.DrawText(text, 
                    new Vector2(_drawingPanel.Size.Width / 2 - text.Size.X / 2, 75));
                
                
                if (Player.ApplicationOwner is {IsDrawer: true})
                {
                    _toolPanel.OnUpdate();
                    
                    text = new Text
                    {
                        Font = AssetManager.GetDefaultFont(48),
                        FontSize = 48f,
                        Value = GameManager.CurrentWord,
                        FontColor = Color.BLACK
                    };
                    Renderer.DrawText(text, 
                        new Vector2(_drawingPanel.Size.Width / 2 - text.Size.X / 2, 25));
                }
            }
            else
            {
                var nickname = NetworkManager.Players[GameManager.Guesser].Nickname;

                var text = new Text
                {
                    Font = AssetManager.GetDefaultFont(48),
                    FontSize = 48f,
                    Value = $"{nickname} guessed right.\nThe word is {GameManager.CurrentWord}",
                    FontColor = Color.BLACK
                };
                Renderer.DrawText(text, 
                    new Vector2(_drawingPanel.Size.Width / 2 - text.Size.X / 2, 25));
            
                if (Player.ApplicationOwner is {IsDrawer: true})
                    _button.OnUpdate();
            }
        }
        else
        {
            var text = new Text
            {
                Font = AssetManager.GetDefaultFont(48),
                FontSize = 48f,
                Value = $"Round ends.\nThe word is {GameManager.CurrentWord}",
                FontColor = Color.BLACK
            };
            Renderer.DrawText(text, 
                new Vector2(_drawingPanel.Size.Width / 2 - text.Size.X / 2, 25));
            
            if (Player.ApplicationOwner is {IsDrawer: true})
                _button.OnUpdate();
        }

        _chatPanel.OnUpdate();
    }

    public override void OnImGuiUpdate()
    {
        
    }

    public override void Dispose()
    {
        _toolPanel.Dispose();
        _chatPanel.Dispose();
        GC.SuppressFinalize(this);
    }
}