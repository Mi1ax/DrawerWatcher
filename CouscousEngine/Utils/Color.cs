using System.Numerics;

namespace CouscousEngine.Utils;

public struct Color
{
    public static readonly Color Empty = new(0, 0, 0, 0);
    public static readonly Color BLACK = new(0, 0, 0);
    public static readonly Color WHITE = new(255, 255, 255);
    public static readonly Color RED = new(255, 0, 0);
    public static readonly Color GRAY = new(130, 130, 130);
    public static readonly Color GREEN = new(0, 255, 0);
    
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public static implicit operator Raylib_CsLo.Color(Color color) 
        => new(color.R, color.G, color.B, color.A);
    
    public static implicit operator Color(System.Drawing.Color color) 
        => new(color.R, color.G, color.B, color.A);
    
    public static explicit operator Color(Raylib_CsLo.Color color) 
        => new(color.r, color.g, color.b, color.a);

    public static implicit operator Vector3(Color color) => new(color.R / 255f, color.G / 255f, color.B / 255f);
    public static explicit operator Color(Vector3 color) => new(
        (byte)(color.X * 255), 
        (byte)(color.Y * 255), 
        (byte)(color.Z * 255)
        );

}