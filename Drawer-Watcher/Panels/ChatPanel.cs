using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.GUI;
using CouscousEngine.Utils;
using Drawer_Watcher.Managers;
using Color = CouscousEngine.Utils.Color;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace Drawer_Watcher.Panels;

public class ChatPanel : IDisposable
{
    public bool DisableInput = false;
    
    private static readonly List<string> _chat = new();

    private readonly Rectangle _bounds;
    private readonly Entry _inputBox;
    
    public ChatPanel(Rectangle bounds)
    {
        _bounds = bounds;
        _inputBox = new Entry(new Rectangle(
            new Size(_bounds.Size.Width - 55, 45), 
            new Vector2(
                _bounds.Position.X + _bounds.Size.Width / 2 - (_bounds.Size.Width - 55) / 2, 
                _bounds.Size.Height - 60
            )))
        {
            OnEnterPressed = (sender, args) =>
            {
                if (Player.ApplicationOwner == null) return;
                MessageHandlers.SendMessageInChat(Player.ApplicationOwner.ID, _inputBox.Text);
                _inputBox.Text = "";
            }
        };
    }

    public static void AddMessage(string nickname, string text)
    {
        _chat.Add($"{nickname}: {text}");
    }
    
    public void OnUpdate()
    {
        Renderer.DrawRectangle(_bounds.Size, _bounds.Position, (Color)_rl.Fade(Color.GRAY, 0.4f));
        _rl.DrawLine(930, 0, 930, 720, Color.BLACK);

        _inputBox.IsEnable = !DisableInput;

        var count = 0;
        foreach (var (id, player) in NetworkManager.Players)
        {
            var text = new Text
            {
                Font = AssetManager.GetDefaultFont(24),
                FontColor = Color.BLACK,
                FontSize = 24,
                Value = $"{player.Nickname}: {player.Score}"
            };
            Renderer.DrawText(text, new Vector2(
                _bounds.Position.X + 10,
                _bounds.Position.Y + 32 * count
                ));
            count++;
        }

        _inputBox.OnUpdate();
        
        for (var i = 0; i < _chat.Count; i++)
        {
            var textSize = _rl.MeasureTextEx(_inputBox.Font, _chat[i], 24f, 1f);
            var position = _inputBox.Position with { Y = _inputBox.Position.Y + textSize.Y * i - textSize.Y * _chat.Count - 5 };
            
            _rl.DrawTextEx(_inputBox.Font, _chat[i], position, 24f, 1f, Color.BLACK);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}