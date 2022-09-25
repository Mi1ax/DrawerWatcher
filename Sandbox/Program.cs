using System.Diagnostics;
using CouscousEngine.Core;
using CouscousEngine.ECS;
using CouscousEngine.Editor;
using CouscousEngine.rlImGui;
using CouscousEngine.Utils;

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

    private class NameComponent
    {
        public string Name { get; set; }

        public NameComponent(string name)
        {
            Name = name;
        }
    }
    
    private static void Main()
    {
        using var sandbox = new Sandbox();
        sandbox.Run();
    }
}