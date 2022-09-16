using System.Numerics;
using CouscousEngine.Utils;

namespace CouscousEngine.Shapes;

public interface IShape : ICloneable
{
    public void Draw();
    
    public Vector2 GetPosition();
    public Color GetColor();
    public Color SetColor(Color color);

    #region Collision

    public bool CheckCollision(Vector2 point);
    public bool CheckCollision(IShape anotherShape);

    #endregion
}