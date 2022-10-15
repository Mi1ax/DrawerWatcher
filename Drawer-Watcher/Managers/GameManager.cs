using System.Numerics;
using CouscousEngine.Core;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;

namespace Drawer_Watcher.Managers;

public struct ConnectionInfo
{
    public string Ip;
    public int Port;

    public ConnectionInfo(string ip, int port)
    {
        Ip = ip;
        Port = port;
    }
    
    public const int MaxConnection = 4;

    public static readonly ConnectionInfo Default = new("46.138.252.180", 34827);
    public static readonly ConnectionInfo Local = new("127.0.0.1", 34827);
}

public struct GameData : IDisposable
{
    public static readonly Color ClearColor = Color.WHITE;

    public static RenderTexture? Painting = null;

    public void Dispose()
    {
        if (Painting != null) 
            Renderer.UnloadRenderTexture(Painting.Value);
    }
}