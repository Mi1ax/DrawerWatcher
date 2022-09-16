namespace CouscousEngine.Utils;

public struct Color
{
    public static readonly Color BLACK = new(0, 0, 0);
    public static readonly Color WHITE = new(255, 255, 255);
    public static readonly Color RED = new(255, 0, 0);
    public static readonly Color GRAY = new(130, 130, 130);
    
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
}