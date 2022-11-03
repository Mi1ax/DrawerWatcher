using CouscousEngine.Core;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;
using MouseButton = CouscousEngine.Core.MouseButton;

namespace Sandbox;

internal static class Program
{
    private class SecondLayer : Layer
    {
        private readonly Rectangle _rectangle;
        private Color _color = Color.BLACK;
        
        public SecondLayer() 
            : base("SecondLayer")
        {
            _rectangle = new Rectangle(80, 15, 125, 25);
        }
        
        public override void OnUpdate(float deltaTime)
        {
            _rl.DrawRectangleRec(_rectangle, _color);
        }

        public override bool OnEvent()
        {
            if (!_rl.CheckCollisionPointRec(Input.GetMousePosition(), _rectangle)) return false;
            if (!Input.IsMouseButtonPressed(MouseButton.LEFT)) return false;
            _color = _color == Color.BLACK ? Color.GRAY : Color.BLACK;
            return true;
        }
    } 
    
    private class SandboxLayer : Layer
    {
        private readonly Rectangle _rectangle;
        private Color _color = Color.RED;
        
        public SandboxLayer() 
            : base("SandboxLayer")
        {
            _rectangle = new Rectangle(10, 10, 125, 25);
        }

        public override void OnUpdate(float deltaTime)
        {
            _rl.DrawRectangleRec(_rectangle, _color);
        }

        public override bool OnEvent()
        {
            if (!_rl.CheckCollisionPointRec(Input.GetMousePosition(), _rectangle)) return false;
            if (!Input.IsMouseButtonPressed(MouseButton.LEFT)) return false;
            _color = _color == Color.RED ? Color.GREEN : Color.RED;
            return true;
        }
    }
    
    private class Sandbox : Application
    {
        public Sandbox() 
            : base("Sandbox")
        {
            PushLayer(new SandboxLayer());
            PushLayer(new SecondLayer());
        }
    }
    
    private static void Main()
    {
        using var snd = new Sandbox();
        snd.Run();
    }
}