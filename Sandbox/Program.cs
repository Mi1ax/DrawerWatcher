using CouscousEngine.Core;
using CouscousEngine.rlImGui;
using CouscousEngine.Utils;

namespace Sandbox;

internal static class Program
{
    private class Sandbox : Application
    {
        public Sandbox() 
            : base("Sandbox")
        {
            
        }

        protected override void Update()
        {
            Renderer.BeginDrawing();
            Renderer.ClearBackground(Color.BLACK);
            
            rlImGui.Begin();
            rlImGui.End();
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