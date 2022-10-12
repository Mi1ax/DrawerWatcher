using CouscousEngine.Networking;
using CouscousEngine.Utils;

namespace CouscousEngine.Core;

public abstract class Application : IDisposable
{
    private static Application? _instance;

    public static Application Instance =>
        _instance ?? throw new NullReferenceException("Application is null");
    
    protected readonly Window Window;

    protected Application(
        string title, 
        int width = 1280, 
        int height = 720
    )
    {
        _instance = this;
        Window = new Window(new WindowData(
            title, 
            width, 
            height)
        );
        rlImGui.rlImGui.Setup();
        AssetManager.Initialize();
    }

    protected abstract void Update();

    protected abstract void OnExit();

    public void Exit() => Dispose();
    
    public Size GetSize() => new (Window.Width, Window.Height);
    
    public void Run()
    {
        while (Window.IsRunning())
            Update();
    }

    public void Dispose()
    {
        OnExit();
        AssetManager.Deinitialize();
        Window.Dispose();
        ClientManager.Client?.Disconnect();
        GC.SuppressFinalize(this);
    }
}