using CouscousEngine.Core;
using CouscousEngine.rlImGui;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Screens;

namespace Drawer_Watcher;

internal class Sandbox : Application
{
    public Sandbox() 
        : base("Drawer Watcher")
    {
        //BIG TODO: Set server on different thread
        NetworkLogger.Init();
        NetworkManager.Initialize();
        GameLogic.Initialize();
        
        AssetManager.LoadFont("RobotoMono-Regular", "Assets/Fonts/RobotoMono-Regular.ttf");
        
        ScreenManager.NavigateTo(new MenuScreen());
    }
    
    protected override void Update()
    {
        NetworkManager.Update();

        Renderer.BeginDrawing();
        Renderer.ClearBackground(GameData.ClearColor);
        {
            ScreenManager.Update();

            rlImGui.Begin();
            NetworkLogger.UpdateImGuiConsole();
            ScreenManager.UpdateImGui();
            rlImGui.End();

            //Renderer.DrawFPS();
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