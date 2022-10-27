using System.Drawing;
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
        
        AssetManager.LoadTexture("Bomb", "Assets/bomb.png");
        
        var font24 = AssetManager.LoadFont("RobotoMono-Regular-24", "Assets/Fonts/RobotoMono-Regular.ttf");
        var font32 = AssetManager.LoadFont("RobotoMono-Regular-32", "Assets/Fonts/RobotoMono-Regular.ttf", 32);
        var font48 = AssetManager.LoadFont("RobotoMono-Regular-48", "Assets/Fonts/RobotoMono-Regular.ttf", 48);
        
        AssetManager.SetDefaultFont(font24!.Value, 24);
        AssetManager.SetDefaultFont(font32!.Value, 32);
        AssetManager.SetDefaultFont(font48!.Value, 48);
        
        ScreenManager.NavigateTo(new MenuScreen());
    }
    
    protected override void Update()
    {
        NetworkManager.Update();

        Renderer.BeginDrawing();
        Renderer.ClearBackground(ColorTranslator.FromHtml("#085EFB"));
        {
            ScreenManager.Update();
            ScreenManager.MenuUpdate();

            rlImGui.Begin();
            ScreenManager.UpdateImGui();
            rlImGui.End();
            
            if (Input.IsKeyPressed(KeyboardKey.ESCAPE))
                ScreenManager.OpenMenu();

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