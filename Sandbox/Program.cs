using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.rlImGui;
using ImGuiNET;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;
using KeyboardKey = CouscousEngine.Core.KeyboardKey;
using MouseButton = CouscousEngine.Core.MouseButton;

namespace Sandbox;

internal static class Program
{
    private class Sandbox : Application
    {
        private readonly RenderTexture _renderTexture;

        private Vector2 _currPoint = Vector2.Zero;
        private Vector2 _prevPoint = Vector2.Zero;

        private float _thickness = 8f;

        public Sandbox() 
            : base("Sandbox")
        {
            _renderTexture = Renderer.LoadRenderTexture(1280, 720);
        }

        private void DrawLine(Vector2 start, Vector2 end)
        {
            var dx = end.X - start.X;
            var dy = end.Y - start.Y;
            var distance = Math.Max(Math.Abs(dx), Math.Abs(dy));
            for (var i = 0; i < distance; i++)
            {
                var x = (int)(start.X + i / distance * dx);
                var y = (int)(start.Y + i / distance * dy);
                _rl.DrawCircleV(new Vector2(x, y), _thickness, Color.WHITE);
            }
        }

        protected override void Update()
        {
            Renderer.BeginDrawing();
            Renderer.ClearBackground(Color.BLACK);

            _currPoint = _rl.GetMousePosition();

            if (Input.IsMouseButtonDown(MouseButton.LEFT))
            {
                Renderer.BeginTextureMode(_renderTexture);
                {
                    //_rl.DrawLineEx(_prevPoint, _currPoint, _thickness, Color.WHITE);
                    DrawLine(_prevPoint, _currPoint);
                }
                Renderer.EndTextureMode();
            }
            
            _prevPoint = _currPoint;

            if (Input.IsKeyPressed(KeyboardKey.SPACE))
            {
                Renderer.BeginTextureMode(_renderTexture);
                Renderer.ClearBackground(Color.BLACK);
                Renderer.EndTextureMode();
            }
            

            Renderer.DrawTexture(_renderTexture, Vector2.Zero, Color.WHITE);     
            
            rlImGui.Begin();
            ImGui.Begin("Status");
            ImGui.Text($"FPS: {_rl.GetFPS()}");
            ImGui.Text($"Mouse Position: [{Input.GetMousePosition().X}:{Input.GetMousePosition().Y}]");
            ImGui.Text($"Mouse Current Position: [{_currPoint.X}:{_currPoint.Y}]");
            ImGui.Text($"Mouse Previous Position: [{_prevPoint.X}:{_prevPoint.Y}]");
            ImGui.DragFloat("Thickness", ref _thickness);
            ImGui.End();
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