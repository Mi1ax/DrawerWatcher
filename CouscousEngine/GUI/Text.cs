using System.Numerics;
using Raylib_CsLo;

namespace CouscousEngine.GUI;

public struct Text
{
    public Font Font { get; init; }
    public float FontSize { get; init; }
    
    public Color FontColor { get; init; }
    
    public string Value { get; set; }
    public Vector2 Size => _rl.MeasureTextEx(Font, Value, FontSize, 1f);
}