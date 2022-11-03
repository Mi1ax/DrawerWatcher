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

    private static void NewWord()
    {
        MessageHandlers.GetNewWord();
        MessageHandlers.ClearPainting();
        GameManager.Guesser = 0;
    }

    public override void OnUpdate(float deltaTime)
    {
        if (!NetworkManager.IsClientConnected || NetworkManager.Players.Count == 0) return;

        Vector2 textSize;

        Renderer.DrawRectangleLines(_drawingPanel, 1f, Color.BLACK);
        
        if (GameManager.Guesser == 0)
        {
            foreach (var player in NetworkManager.Players.Values)
                        player.Update();
                    
            Renderer.DrawTexture(GameData.Painting!.Value, _drawingPanel.Position, Color.WHITE);

            if (Player.ApplicationOwner is {IsDrawer: true})
            {
                _toolPanel.OnUpdate();
        
                textSize = _rl.MeasureTextEx(
                    AssetManager.GetDefaultFont(48), 
                    GameManager.CurrentWord, 48f, 1f
                );
        
                _rl.DrawTextEx(AssetManager.GetDefaultFont(48), 
                    GameManager.CurrentWord, 
                    new Vector2(
                        _drawingPanel.Size.Width / 2 - textSize.X,
                        25
                    ), 
                    48f, 1f, Color.BLACK);
            }
        }
        else
        {
            var nickname = NetworkManager.Players[GameManager.Guesser].Nickname;
            
            textSize = _rl.MeasureTextEx(
                AssetManager.GetDefaultFont(48), 
                $"{nickname} guessed right", 48f, 1f
            );
        
            _rl.DrawTextEx(AssetManager.GetDefaultFont(48), 
                $"{nickname} guessed right", 
                new Vector2(
                    _drawingPanel.Size.Width / 2 - textSize.X / 2,
                    25
                ), 
                48f, 1f, Color.BLACK);
            
            if (Player.ApplicationOwner is {IsDrawer: true})
                _button.OnUpdate(deltaTime);
        }

        _chatPanel.OnUpdate(deltaTime);
    }

    public override bool OnEvent()
    {
        return _button.OnEvent();
    }
}