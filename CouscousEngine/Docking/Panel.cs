using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Shapes;
using CouscousEngine.Utils;

namespace CouscousEngine.Docking;

public class Panel
{
    private Rectangle _rectangle;

    public Vector2 Position => _rectangle.Position;
    public Size Size => _rectangle.Size;

    public Color Color
    {
        get => _rectangle.Color;
        set => _rectangle.Color = value;
    }
    
    public Panel(Rectangle rectangle)
    {
        _rectangle = rectangle;
    }
    
    public void Draw()
    {
        _rectangle.Draw();
    }
}