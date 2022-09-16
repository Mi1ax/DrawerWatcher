﻿using Raylib_CsLo;
using rl = Raylib_CsLo.Raylib;

namespace CouscousEngine.Core;

public struct WindowData
{
    public string Title { get; set; }
    
    public uint Width { get; set; }
    public uint Height { get; set; }

    public WindowData(string title, uint width, uint height)
    {
        Title = title;
        
        Width = width;
        Height = height;
    }
}

public class Window : IDisposable
{
    private readonly WindowData _data;

    public Window(WindowData data)
    {
        _data = data;
        rl.SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT);
        rl.InitWindow((int)_data.Width, (int)_data.Height, _data.Title);
    }

    public uint Width => _data.Width;
    public uint Height => _data.Height;

    public bool IsRunning() => !rl.WindowShouldClose();
    
    public void Dispose()
    {
        rl.CloseWindow();
        GC.SuppressFinalize(this);
    }
}