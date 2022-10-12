using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.GUI;
using CouscousEngine.Utils;
using Color = CouscousEngine.Utils.Color;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace Drawer_Watcher.Panels;

public class ChatPanel : IDisposable
{
    private static List<string> _chat = new();

    private readonly Rectangle _bounds;
    private readonly InputBox _inputBox;
    
    public ChatPanel(Rectangle bounds)
    {
        _bounds = bounds;
        _inputBox = new InputBox(new Rectangle(
            new Size(_bounds.Size.Width - 55, 45), 
            new Vector2(
                _bounds.Position.X + _bounds.Size.Width / 2 - (_bounds.Size.Width - 55) / 2, 
                _bounds.Size.Height - 60
            )));
        
        _inputBox.SetAction(() =>
        {
            if (Player.ApplicationOwner == null) return;
            Player.SendMessageInChat(Player.ApplicationOwner.ID, _inputBox.Text);
            _inputBox.Text = "";
        });
    }

    public static void AddMessage(ushort senderID, string text)
    {
        _chat.Add($"{text}: {senderID}");
    }
    
    public void OnUpdate()
    {
        Renderer.DrawRectangle(_bounds.Size, _bounds.Position, (Color)_rl.Fade(Color.GRAY, 0.4f));
        _rl.DrawLine(930, 720 - 144, 930, 720, Color.BLACK);
        _inputBox.OnUpdate();

        for (var i = 0; i < _chat.Count; i++)
        {
            var textSize = _rl.MeasureTextEx(_inputBox.Font, _chat[i], 24f, 1f);
            var position = new Vector2(
                _inputBox.Position.X + _inputBox.Size.Width - textSize.X, 
                _inputBox.Position.Y + textSize.Y * i - textSize.Y * _chat.Count - 5
            );
            
            _rl.DrawTextEx(_inputBox.Font, _chat[i], position, 24f, 1f, Color.BLACK);
        }
    }

    public void Dispose()
    {
        _inputBox.Dispose();
        GC.SuppressFinalize(this);
    }
}