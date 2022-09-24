using System.Numerics;
using CouscousEngine.CCGui;
using CouscousEngine.Utils;

namespace CouscousEngine.GUI;

public abstract class Visual
{
    [Inspectable] protected Size Size { get; set; }
    [Inspectable] protected Vector2 Position { get; set; }

    protected Visual()
    {
        Size = Size.Zero;
        Position = Vector2.Zero;
    }
    
    protected Visual(Size size, Vector2 position)
    {
        Size = size;
        Position = position;
    }
    
    public abstract void Update();
}