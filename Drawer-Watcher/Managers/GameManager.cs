using System.Numerics;
using CouscousEngine.Core;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;

namespace Drawer_Watcher.Managers;

public static class ConnectionInfo
{
    public static string Ip = "46.138.252.180";
    public static int Port = 34827;

    public const int MaxConnection = 4;
}

public enum MessageID : ushort
{
    SendPainting = 1,
    SendPosition,
    DrawerChanged,
    AllClear,
    
    ChatMessage
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

public static class GameManager
{
    public static readonly Dictionary<ushort, Player> Players = new();
}