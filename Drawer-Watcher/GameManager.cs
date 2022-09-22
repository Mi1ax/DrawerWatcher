using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Networking;
using Raylib_CsLo;
using Riptide;
using Color = CouscousEngine.Utils.Color;

namespace Drawer_Watcher;

public static class ConnectionInfo
{
    public const string Ip = "46.138.252.180";
    public const ushort Port = 34827;

    public const int MaxConnection = 2;
}

public enum MessageID : ushort
{
    SendPainting = 1,
    SendPosition,
    DrawerChanged,
}

public struct GameData : IDisposable
{
    public static readonly Color ClearColor = Color.BLACK;

    public static readonly RenderTexture Painting = Renderer.LoadRenderTexture(
        (int)Application.Instance.GetSize().Width,
        (int)Application.Instance.GetSize().Height);


    public void Dispose() => Renderer.UnloadRenderTexture(Painting);
}

public static class GameManager
{
    public static readonly Dictionary<ushort, Player> Players = new();

    public static bool IsConnectedToServer => _isHost || ClientManager.Client!.IsConnected;

    //private static RenderTexture _painting;
    
    public static void DrawPainting() => Renderer.DrawTexture(GameData.Painting, Vector2.Zero, Color.WHITE);
    
    #region Server
    
    private static bool _isHost;

    public static bool IsHost
    {
        get => _isHost;
        set
        {
            _isHost = value;
            if (_isHost) InitializeServer();
            else CloseServer();
        }
    }

    private static void InitializeServer()
    {
        ServerManager.Initialize(
            (_, e) =>
            {
                new Player(e.Client.Id);
            },
            (_, e) =>
            {
                Players.Remove(e.Client.Id);
            }
        );
        ServerManager.Start(ConnectionInfo.Port, ConnectionInfo.MaxConnection);
    }

    private static void CloseServer()
    {
        ServerManager.Stop();
    }
    
    #endregion

    #region Client

    private static bool _isConnect;
    
    public static void Initialize()
    {
        ClientManager.Initialize((_, e) =>
        {
            Players.Remove(e.Id);
        });
    }

    public static bool Connect()
    {
        _isConnect = ClientManager.Connect(ConnectionInfo.Ip, ConnectionInfo.Port);
        return _isConnect;
    }
    
    #endregion
    
    public static void Update()
    {
        if (IsHost) ServerManager.Update();
        ClientManager.Update();
    }
}