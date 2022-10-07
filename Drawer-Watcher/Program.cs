using CouscousEngine.Core;
using Drawer_Watcher.Screens;

namespace Drawer_Watcher;

internal class Sandbox : Application
{
    public Sandbox() 
        : base("Sandbox")
    {
        ScreenManager.NavigateTo(new MenuScreen());
            
        GameManager.Initialize();
    }
    
    protected override void Update()
    {
        GameManager.Update();
        
        Renderer.BeginDrawing();
        Renderer.ClearBackground(GameData.ClearColor);
        {
            GameManager.DrawPainting();
            
            ScreenManager.Update();
            
            Renderer.DrawFPS();
        }
        Renderer.EndDrawing();
    }

    protected override void OnExit()
    {
        
    }
}

internal static class Program
{
    private static void Main()
    {
        using var app = new Sandbox();
        app.Run();
    }
}