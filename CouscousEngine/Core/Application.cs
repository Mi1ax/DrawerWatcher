using CouscousEngine.Networking;
using CouscousEngine.Utils;

namespace CouscousEngine.Core;

public abstract class Application : IDisposable
{
    #region Instance

    private static Application? _instance;

    public static Application Instance =>
        _instance ?? throw new NullReferenceException("Application is null");

    #endregion
    
    protected readonly Window Window;

    public Size WindowSize => new (Window.Width, Window.Height);

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
    }

    protected abstract void Update();

    protected abstract void OnExit();

    public void Exit() => Dispose();
    
    public void Run()
    {
        while (Window.IsRunning())
            Update();
    }

    public void Dispose()
    {
        OnExit();
        
        AssetManager.Deinitialize();
        ClientManager.Client?.Disconnect();
        
        Window.Dispose();
        GC.SuppressFinalize(this);
    }
}