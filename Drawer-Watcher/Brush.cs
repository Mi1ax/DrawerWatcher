using System.Numerics;
using CouscousEngine.Utils;
using Drawer_Watcher.Managers;

namespace Drawer_Watcher;

public struct Brush
{
    public Vector3 Color { get; set; }
    public float Thickness { get; set; }

    public static Color ClearColor => GameData.ClearColor;

    public static readonly Brush Default = new() {Color = Vector3.Zero, Thickness = 8f};
}