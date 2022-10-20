using System.Drawing;
using CouscousEngine.Core;
using CouscousEngine.GUI;
using Color = CouscousEngine.Utils.Color;
using Rectangle = Raylib_CsLo.Rectangle;

namespace Sandbox;

internal static class Program
{
    private class Sandbox : Application
    {
        private readonly Button _button;
        
        public Sandbox() 
            : base("Sandbox")
        {
            AssetManager.LoadFont("RobotoMono-Regular-24", "Assets/Fonts/RobotoMono-Regular.ttf");

            _button = new Button(new Rectangle(50, 50, 125, 35))
            {
                Text = "Connect",
                FontSize = 24,
                FontColor = Color.BLACK,

                CornerRadius = 0.65f,
                BorderThickness = 3f,
                BorderColor = Color.BLACK,

                Color = ColorTranslator.FromHtml("#FFBF00"),
                OnButtonClick = (sender, args) =>
                {
                    Console.WriteLine("Connect");
                }
            };
        }

        protected override void Update()
        {
            Renderer.BeginDrawing();
            Renderer.ClearBackground(Color.WHITE);

            _button.OnUpdate();
            
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