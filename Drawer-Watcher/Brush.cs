using System.Drawing;
using System.Numerics;
using Drawer_Watcher.Managers;
using Color = CouscousEngine.Utils.Color;

namespace Drawer_Watcher;

public struct Brush
{
    public Vector3 Color { get; set; }
    public float Thickness { get; set; }

    public static Color ClearColor => GameData.ClearColor;

    public static readonly Brush Default = new()
    {
        Color = (Color)ColorTranslator.FromHtml("#414042"), 
        Thickness = 8f
    };
}