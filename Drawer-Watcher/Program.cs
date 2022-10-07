using CouscousEngine.Core;
using Drawer_Watcher.Screens;

namespace Drawer_Watcher;

internal class Sandbox : Application
{
    public Sandbox() 
        : base("Drawer Watcher")
    {
        //BIG TODO: Set server on different thread
        ScreenManager.NavigateTo(new MenuScreen());
            
        NetworkManager.Initialize();
    }
    
    protected override void Update()
    { 
        NetworkManager.Update();
        
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