using CouscousEngine.Networking;
using CouscousEngine.rlImGui;
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
        PushLayer(new ImGuiLayer());
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
            for (var i = _layerStack.Layers.Count - 1; i >= 0; i--)
            {
                _layerStack.Layers[i].OnUpdate(_rl.GetFrameTime());
            }
            
            rlImGui.rlImGui.Begin();
            foreach (var layer in _layerStack.Layers)
            {
                layer.OnImGuiUpdate();
            }
            rlImGui.rlImGui.End();
            
            for (var i = 0; i < _layerStack.Layers.Count; i++)
            {
                if (_layerStack.Layers[i].OnEvent())
                    break;
            }

            Renderer.EndDrawing();
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