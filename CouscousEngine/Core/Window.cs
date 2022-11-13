using Raylib_CsLo;

namespace CouscousEngine.Core;

public struct WindowData
{
    public string Title { get; set; }
    
    public int Width { get; set; }
    public int Height { get; set; }
    public bool DisableWindowCloseButton { get; set; }

    public WindowData(string title, int width, int height, bool disableWindowCloseButton = false)
    {
        Title = title;

        Width = width;
        Height = height;
        DisableWindowCloseButton = disableWindowCloseButton;
    }
}

public class Window : IDisposable
{
    private readonly WindowData _data;

    public Window(WindowData data)
    {
        _data = data;
        _rl.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT);
        _rl.InitWindow(_data.Width, _data.Height, _data.Title);
        _rl.SetExitKey(0);
    }

    public int Width => _data.Width;
    public int Height => _data.Height;

    public bool IsRunning() 
        => _data.DisableWindowCloseButton || !_rl.WindowShouldClose();

    public static void SetTargetFPS(int fps) => _rl.SetTargetFPS(fps);
    
    public void Dispose()
    {
        _rl.CloseWindow();
        GC.SuppressFinalize(this);
    }
}