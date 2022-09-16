namespace CouscousEngine.Utils;

public struct Color
{
    public static readonly Color BLACK = new(0, 0, 0);
    public static readonly Color WHITE = new(255, 255, 255);
    public static readonly Color RED = new(255, 0, 0);
    
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
    
    public Raylib_CsLo.Color ToRayColor() => new(R, G, B, A);
}