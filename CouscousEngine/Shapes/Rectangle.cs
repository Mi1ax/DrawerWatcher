using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Utils;

namespace CouscousEngine.Shapes;

public class Rectangle : Shape
{
    public Size Size { get; set; }

    public Rectangle()
    {
        Size = Size.Zero;
    }
    
    public Rectangle(Size size, Vector2 position, Color color) 
        : base(position, color)
    {
        Size = size;
    }
    
    public Rectangle(Size size, Vector2 position) 
        : base(position, Color.WHITE)
    {
        Size = size;
    }

    public override void Draw()
    {
        Renderer.DrawRectangle(Size, Position, Color);
    }
    
    public override bool CheckCollision(Vector2 point)
    {
        throw new NotImplementedException();
    }

    public override bool CheckCollision(Shape anotherShape)
    {
        throw new NotImplementedException();
    }
    
    public static implicit operator Raylib_CsLo.Rectangle(Rectangle r) 
        => new(r.Position.X, r.Position.Y, r.Size.Width, r.Size.Height);
}