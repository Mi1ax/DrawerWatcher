using System.Numerics;
using CouscousEngine.Utils;

namespace CouscousEngine.Shapes;

public abstract class Shape : ICloneable
{
    protected Vector2 Position;
    protected Color Color;
    
    protected Shape(Vector2 position, Color color)
    {
        Position = position;
        Color = color;
    }
    
    public abstract void Draw();

    public Vector2 GetPosition() => Position;
    public Color GetColor() => Color;
    public Color SetColor(Color color) => Color = color;

    #region Collision

    public abstract bool CheckCollision(Vector2 point);
    public abstract bool CheckCollision(Shape anotherShape);

    #endregion

    public object Clone() => MemberwiseClone();
}