using CouscousEngine.Utils;
using Drawer_Watcher.Managers;

namespace Drawer_Watcher;

public struct Brush
{
    public Color Color { get; set; }
    public float Thickness { get; set; }

    public static Color ClearColor => GameData.ClearColor;

    public static readonly Brush Default = new() {Color = Color.BLACK, Thickness = 16f};
}