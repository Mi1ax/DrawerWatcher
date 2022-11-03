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

    private readonly LayerStack _layerStack;

    public readonly Window Window;

    public Size WindowSize => new (Window.Width, Window.Height);

    protected Application(
        string title, 
        int width = 1280, 
        int height = 720
    )
    {
        _instance = this;
        _layerStack = new LayerStack();
        Window = new Window(new WindowData(
            title, 
            width, 
            height)
        );
        rlImGui.rlImGui.Setup();
    }

    protected virtual void OnExit() {}

    public void Exit() => Dispose();

    protected void PushLayer(Layer layer)
    {
        _layerStack.PushLayer(layer);
        layer.OnAttach();
    }
    
    public void Run()
    {
        while (Window.IsRunning())
        {
            Renderer.BeginDrawing();
            Renderer.ClearBackground(Color.WHITE);
            foreach (var layer in _layerStack.Layers)
            {
                layer.OnImGuiUpdate();
                layer.OnUpdate(_rl.GetFrameTime());
            }
            Renderer.EndDrawing();

            for (var i = _layerStack.Layers.Count - 1; i >= 0; i--)
            {
                if (_layerStack.Layers[i].OnEvent())
                    break;
            }
        }
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