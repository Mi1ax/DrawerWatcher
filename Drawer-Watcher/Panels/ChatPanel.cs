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
            MessageHandlers.SendMessageInChat(Player.ApplicationOwner.ID, _inputBox.Text);
            _inputBox.Text = "";
        });
    }

    public static KeyValuePair<ushort, string> GetLastMessage()
    {
        if (_chat.Count == 0) return new KeyValuePair<ushort, string>(0, "");
        var lastStr = _chat.Last().Split(':');
        return new KeyValuePair<ushort, string>(
            Convert.ToUInt16(lastStr[1]), 
            lastStr[0]
            );
    }

    public static void AddMessage(ushort senderID, string text)
    {
        _chat.Add($"{text}: {senderID}");
    }
    
    public void OnUpdate()
    {
        Renderer.DrawRectangle(_bounds.Size, _bounds.Position, (Color)_rl.Fade(Color.GRAY, 0.4f));
        _rl.DrawLine(930, 720 - 144, 930, 720, Color.BLACK);
        if (!DisableInput)
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