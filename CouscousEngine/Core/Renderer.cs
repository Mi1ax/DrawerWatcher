using System.Numerics;
using CouscousEngine.Utils;
using RL = Raylib_CsLo.Raylib;

namespace CouscousEngine.Core;

public static class Renderer
{
    public static void BeginDrawing() => RL.BeginDrawing();
    public static void EndDrawing() => RL.EndDrawing();

    public static void ClearBackground(Color color)
        => RL.ClearBackground(color.ToRayColor());

    public static void DrawCircle(Vector2 position, float radius, Color color)
        => RL.DrawCircleV(position, radius, color.ToRayColor());

    public static void DrawFPS(int x = 15, int y = 15)
        => RL.DrawFPS(x, y);
}