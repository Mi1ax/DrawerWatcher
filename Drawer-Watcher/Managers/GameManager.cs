using System.Numerics;
using CouscousEngine.Core;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;

namespace Drawer_Watcher.Managers;

public static class ConnectionInfo
{
    public static string Ip = "46.138.252.180";
    public static int Port = 34827;

    public const int MaxConnection = 2;
}

public enum MessageID : ushort
{
    SendPainting = 1,
    SendPosition,
    DrawerChanged,
    AllClear
}

public struct GameData : IDisposable
{
    public static readonly Color ClearColor = Color.WHITE;

    public static readonly RenderTexture Painting = Renderer.LoadRenderTexture(
        (int)Application.Instance.GetSize().Width,
        (int)Application.Instance.GetSize().Height);


    public void Dispose() => Renderer.UnloadRenderTexture(Painting);
}

public static class GameManager
{
    public static readonly Dictionary<ushort, Player> Players = new();
    
    public static void DrawPainting() => Renderer.DrawTexture(GameData.Painting, Vector2.Zero, Color.WHITE);
}