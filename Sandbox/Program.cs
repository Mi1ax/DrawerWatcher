using CouscousEngine.Core;
using CouscousEngine.GUI;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;

namespace Sandbox;

internal static class Program
{
    private class Sandbox : Application
    {
        private readonly Entry _entry;

        public Sandbox() 
            : base("Sandbox")
        {
            var font24 = AssetManager.LoadFont("RobotoMono-Regular-24", "Assets/Fonts/RobotoMono-Regular.ttf");
            AssetManager.SetDefaultFont(font24!.Value, 24);
            
            _entry = new Entry(new Rectangle(50, 50, 180, 45))
            {
                BorderColor = Color.BLACK,
                BordeThickness = 2f,
                Color = Color.WHITE,
                CornerRadius = 0.65f
            };
        }

        protected override void Update()
        {
            Renderer.BeginDrawing();
            Renderer.ClearBackground(Color.WHITE);

            _entry.OnUpdate();
            
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