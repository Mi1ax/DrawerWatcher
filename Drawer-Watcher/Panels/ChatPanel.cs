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
        _inputBox = new InputBox(new Rectangle(new Size(145, 45), new Vector2(100)));
    }

    public void OnUpdate()
    {
        Renderer.DrawRectangle(_bounds.Size, _bounds.Position, (Color)_rl.Fade(Color.GRAY, 0.4f));
        _inputBox.OnUpdate();
    }
}