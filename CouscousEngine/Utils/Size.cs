using System.Numerics;

namespace CouscousEngine.Utils;

public struct Size
{
    public static readonly Size Zero = new(0, 0);
    
    public float Width { get; set; }
    public float Height { get; set; }

    public Size(float width, float height)
    {
        Width = width;
        Height = height;
    }

    public override string ToString() => $"<{Width}, {Height}>";

    public static implicit operator Vector2(Size size) => new(size.Width, size.Height);
}