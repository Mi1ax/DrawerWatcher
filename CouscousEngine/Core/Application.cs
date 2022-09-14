using static Raylib_CsLo.Raylib;

namespace CouscousEngine.Core;

public abstract class Application : IDisposable
{
    protected readonly Window Window;

    protected Application(
        string title, 
        uint width = 1280, 
        uint height = 720
    ) {
        Window = new Window(new WindowData(
            title, 
            width, 
            height)
        );
    }

    protected abstract void Update();
    protected abstract void Draw();
    
    public void Run()
    {
        while (Window.IsRunning())
        {
            Update();
            BeginDrawing();
            {
                ClearBackground(BLACK);
            }
            EndDrawing();
            
            Draw();
        }
    }

    public void Dispose()
    {
        Window.Dispose();
        GC.SuppressFinalize(this);
    }
}