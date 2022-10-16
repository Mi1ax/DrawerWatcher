using System.Numerics;
using CouscousEngine.Utils;

namespace CouscousEngine.Shapes;

public abstract class Shape : ICloneable
{
    public Vector2 Position;
    public Color Color;
    
    protected Shape()
    {
        Position = Vector2.Zero;
        Color = Color.WHITE;
    }
    
    protected Shape(Vector2 position, Color color)
    {
        Position = position;
        Color = color;
    }

    public Vector2 GetPosition() => Position;
    public Color GetColor() => Color;
    public Color SetColor(Color color) => Color = color;

    #region Collision

    public abstract bool CheckCollision(Vector2 point);
    public abstract bool CheckCollision(Shape anotherShape);

    #endregion

    public object Clone() => MemberwiseClone();
}