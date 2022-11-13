using CouscousEngine.Core;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Screens;
using Drawer_Watcher.Screens.ImGuiWindows;
using Riptide;

namespace Drawer_Watcher;

public class GameLayer : Layer
{
    public GameLayer() 
        : base(nameof(GameLayer))
    {
        
    }

    public override void OnAttach()
    {
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

    public override void OnImGuiUpdate()
    {
        if (_rl.WindowShouldClose())
        {
            MessageBox.Show("Exit", "Are you sure?",
                MessageBoxButtons.YesNo, button =>
                {
                    if (button == MessageBoxResult.Yes)
                        Application.Instance.Close();
                });
        }
        
        MessageBox.OnImGuiUpdate();
        ScreenManager.OnUpdateImGui();
    }

    public override void OnUpdate(float deltaTime)
    {
        NetworkManager.Update();
        ScreenManager.OnUpdate(deltaTime);
    }

    public override bool OnEvent()
    {
        return ScreenManager.OnEvent();
    }
}

internal class Sandbox : Application
{
    public Sandbox() 
        : base("Drawer Watcher", disableWindowCloseButton: true)
    {
        PushLayer(new GameLayer());
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