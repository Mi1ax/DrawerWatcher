using System.Numerics;

namespace CouscousEngine.Utils;

public struct Size
{
    public static readonly Size ZERO = new(0, 0);
    
    public float Width { get; set; }
    public float Height { get; set; }

    public Size(float width, float height)
    {
        Width = width;
        Height = height;
    }
    
    public static implicit operator Vector2(Size size) => new(size.Width, size.Height);
}