using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Utils;

using rl = Raylib_CsLo.Raylib;

namespace CouscousEngine.Shapes;

public class Circle : Shape
{
    public float Radius { get; set; }

    public Circle(
        Vector2 position, 
        float radius, 
        Color color
        ) : base(position, color)
    {
        Radius = radius;
    }
    
    public virtual void Update()
    {
        Renderer.DrawCircle(Position, Radius, Color);
    }

    public float GetRadius() => Radius;

    public override bool CheckCollision(Vector2 point)
        => rl.CheckCollisionPointCircle(point, Position, Radius);

    public override bool CheckCollision(Shape anotherShape)
    {
        throw new NotImplementedException();
    }
}