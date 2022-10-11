using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.rlImGui;
using ImGuiNET;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;
using KeyboardKey = CouscousEngine.Core.KeyboardKey;

namespace Sandbox;

internal static class Program
{
    private class Sandbox : Application
    {
        private string _text;

        private Font _font;
        
        public Sandbox() 
            : base("Sandbox")
        {
            _text = "";
            
            unsafe
            {
                fixed (int* codepoints = new int[512])
                {
                    for (var i = 0; i < 95; i++) 
                        codepoints[i] = 32 + i;   // Basic ASCII characters
                    for (var i = 0; i < 255; i++) 
                        codepoints[96 + i] = 0x400 + i;   // Cyrillic characters
                    _font = _rl.LoadFontEx("Assets/Fonts/RobotoMono-Regular.ttf", 24, codepoints, 512);
                }
            }
        }

        protected override void Update()
        {
            Renderer.BeginDrawing();
            Renderer.ClearBackground(Color.BLACK);

            var key = _rl.GetCharPressed();

            while (key > 0)
            {
                if (key is >= 32 and <= 125 or >= 1040 and <= 1103 && _text.Length < 64)
                    _text += (char)key;
            
                key = _rl.GetCharPressed();
            }
            
            if (Input.IsKeyPressed(KeyboardKey.BACKSPACE) && _text.Length != 0)
            {
                _text = _text[..^1];
            }
            
            _rl.DrawTextEx(_font, _text, new Vector2(100), 24f, 2f, Color.WHITE);
            
            Renderer.EndDrawing();
        }

        protected override void OnExit()
        {
            
        }
    }

    private static void Main()
    {
        using var sandbox = new Sandbox();
        sandbox.Run();
    }
}