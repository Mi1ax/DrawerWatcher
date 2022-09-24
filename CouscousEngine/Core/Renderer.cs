using System.Numerics;
using CouscousEngine.Utils;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace CouscousEngine.Core;

public static class Renderer
{
    public static void BeginDrawing() => _rl.BeginDrawing();
    public static void EndDrawing() => _rl.EndDrawing();

    #region RenderTexture

    public static RenderTexture LoadRenderTexture(int width, int height) 
        => _rl.LoadRenderTexture(width, height);

    public static void UnloadRenderTexture(RenderTexture renderTexture)
        => _rl.UnloadRenderTexture(renderTexture);
    
    public static void BeginTextureMode(RenderTexture texture) => _rl.BeginTextureMode(texture);
    public static void EndTextureMode() => _rl.EndTextureMode();
    public static void DrawTexture(Texture texture, Vector2 position, Color color) 
        => _rl.DrawTextureV(texture, position, color);
    public static void DrawTexture(RenderTexture renderTexture, Vector2 position, Color color) 
        => _rl.DrawTextureRec(
            renderTexture.texture, 
            new Raylib_CsLo.Rectangle(0, 0, renderTexture.texture.width, -renderTexture.texture.height),
            position,
            color);

    #endregion

    public static void ClearBackground(Color color)
        => _rl.ClearBackground(color);

    public static void DrawCircle(Vector2 position, float radius, Color color)
        => _rl.DrawCircleV(position, radius, color);
    
    // TODO: Add thickness
    public static void DrawCircleLines(Vector2 position, float radius, float thickness, Color color)
        => _rl.DrawCircleLines((int)position.X, (int)position.Y, radius, color);

    public static void DrawRectangle(Size size, Vector2 position, Color color)
        => _rl.DrawRectangleV(position, size, color);
    
    public static void DrawRectangleLines(Rectangle rectangle, float thickness, Color color)
        => _rl.DrawRectangleLinesEx(rectangle, thickness, color);
    
    public static void DrawFPS(int x = 15, int y = 15)
        => _rl.DrawFPS(x, y);
}