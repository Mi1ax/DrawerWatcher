using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.GUI;
using CouscousEngine.Utils;
using Color = CouscousEngine.Utils.Color;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace Drawer_Watcher.Panels;

public class ChatPanel
{
    private readonly Rectangle _bounds;
    private readonly InputBox _inputBox;
    
    public ChatPanel(Rectangle bounds)
    {
        _bounds = bounds;
        _inputBox = new InputBox(new Rectangle(
            new Size(_bounds.Size.Width - 55, 45), 
            new Vector2(
                _bounds.Size.Width / 2 - (_bounds.Size.Width - 55) / 2, 
                _bounds.Size.Height - 60
            )));
        
        _inputBox.SetAction(() =>
        {
            _inputBox.Text = "";
        });
    }

    public void OnUpdate()
    {
        Renderer.DrawRectangle(_bounds.Size, _bounds.Position, (Color)_rl.Fade(Color.GRAY, 0.4f));
        _inputBox.OnUpdate();
    }
}