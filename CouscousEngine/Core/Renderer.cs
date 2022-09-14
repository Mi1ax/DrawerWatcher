using Raylib_CsLo;
using RL = Raylib_CsLo.Raylib;

namespace CouscousEngine.Core;

public static class Renderer
{
    public static void BeginDrawing() => RL.BeginDrawing();
    public static void EndDrawing() => RL.EndDrawing();
    
    // TODO: Color
    public static void ClearBackground(float red, float green, float blue, float alpha = 1f) 
        => RL.ClearBackground(new Color(
            (int)(red * 255), 
            (int)(green * 255), 
            (int)(blue * 255), 
            (int)(alpha * 255))
        );
}