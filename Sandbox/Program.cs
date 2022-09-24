using CouscousEngine.CCGui;
using CouscousEngine.Core;
using CouscousEngine.Utils;
using ImGuiNET;

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
            ImGui.ShowDemoWindow();
            rlImGui.End();
            Renderer.EndDrawing();
        }
    }
    
    private static void Main()
    {
        var sandbox = new Sandbox();
        sandbox.Run();
    }
}