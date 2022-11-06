using CouscousEngine.Core;

namespace Sandbox;

internal static class Program
{
    private class Sandbox : Application
    {
        public Sandbox() 
            : base("Sandbox")
        {
            PushLayer(new SandboxLayer());
        }
    }
    
    private static void Main()
    {
        using var snd = new Sandbox();
        snd.Run();
    }
}