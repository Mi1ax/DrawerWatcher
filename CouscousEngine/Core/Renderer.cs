using System.Numerics;
using CouscousEngine.Utils;
using rl = Raylib_CsLo.Raylib;
using _rl = Raylib_CsLo;

namespace CouscousEngine.Core;

public static class Renderer
{
    public static void BeginDrawing() => rl.BeginDrawing();
    public static void EndDrawing() => rl.EndDrawing();

    #region RenderTexture

    public static _rl.RenderTexture LoadRenderTexture(uint width, uint height) 
        => rl.LoadRenderTexture((int)width, (int)height);

    public static void UnloadRenderTexture(_rl.RenderTexture renderTexture)
        => rl.UnloadRenderTexture(renderTexture);
    
    public static void BeginTextureMode(_rl.RenderTexture texture) => rl.BeginTextureMode(texture);
    public static void EndTextureMode() => rl.EndTextureMode();
    public static void DrawTexture(_rl.Texture texture, Vector2 position, Color color) 
        => rl.DrawTextureV(texture, position, color);
    public static void DrawTexture(_rl.RenderTexture renderTexture, Vector2 position, Color color) 
        => rl.DrawTextureRec(
            renderTexture.texture, 
            new _rl.Rectangle(0, 0, renderTexture.texture.width, -renderTexture.texture.height),
            position,
            color);

    #endregion

    public static void ClearBackground(Color color)
        => rl.ClearBackground(color);

    public static void DrawCircle(Vector2 position, float radius, Color color)
        => rl.DrawCircleV(position, radius, color);

    public static void DrawRectangle(Size size, Vector2 position, Color color)
        => rl.DrawRectangleV(position, size, color);
    
    public static void DrawFPS(int x = 15, int y = 15)
        => rl.DrawFPS(x, y);
}