using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Editor;
using CouscousEngine.rlImGui;
using CouscousEngine.Utils;
using ImGuiNET;

namespace Sandbox;

internal static class Program
{
    private class Sandbox : Application
    {
        private readonly Scene _scene;
        
        public Sandbox() 
            : base("Sandbox")
        {
            _scene = Scene.Load("Sandbox");
        }

        protected override void Update()
        {
            Renderer.BeginDrawing();
            Renderer.ClearBackground(Color.BLACK);
            
            _scene.Update();
            
            rlImGui.Begin();
            Inspector.DrawWindow(_scene.Buttons.First().Value);
            rlImGui.End();
            Renderer.EndDrawing();
        }

        protected override void OnExit()
        {
            Scene.Save(_scene, "SimpleScene");
        }
    }
    
    public static Vector2 Position = Vector2.Zero;
    
    private static void Main()
    {
        /*using var sandbox = new Sandbox();
        sandbox.Run();*/
        _rl.InitWindow(1280, 720, "Sandbox");

        var thread = new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep(500);
                Position.X += 10;
            }
        })
        {
            IsBackground = true
        };
        thread.Start();
        while (!_rl.WindowShouldClose())
        {
            _rl.BeginDrawing();
            _rl.ClearBackground(_rl.BLACK);
            _rl.DrawRectangle((int)Position.X, (int)Position.Y, 64, 64, Color.GREEN);
            _rl.EndDrawing();
        }
        
        _rl.CloseWindow();
    }
}