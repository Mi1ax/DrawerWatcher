using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Utils;

using rl = Raylib_CsLo.Raylib;

namespace CouscousEngine.Shapes;

public class Circle : IShape
{
    private Color _color;
    public float Radius { get; set; }
    public Vector2 Position { get; set; }

    public Circle(
        Vector2 position, 
        float radius, 
        Color color
        )
    {
        _color = color;
        Position = position;
        Radius = radius;
    }
    
    public void Draw()
    {
        Renderer.DrawCircle(Position, Radius, _color);
    }

    public float GetRadius() => Radius;
    public Vector2 GetPosition() => Position;
    public Color GetColor() => _color;
    public Color SetColor(Color color) => _color = color;

    public bool CheckCollision(Vector2 point)
        => rl.CheckCollisionPointCircle(point, Position, Radius);

    public bool CheckCollision(IShape anotherShape)
    {
        throw new NotImplementedException();
    }

    public object Clone() => MemberwiseClone();
}