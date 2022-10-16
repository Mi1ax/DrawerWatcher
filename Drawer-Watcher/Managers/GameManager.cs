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

public static class GameManager
{
    public static string CurrentWord = "";
    public static ushort Guesser = 0;

    private static readonly Random _random = new(DateTime.Now.GetHashCode());
    private static readonly string[] EnglishWords = File.ReadAllText("Assets/Words.txt").Split("\n");
    
    public static string GetRandomWord()
    {
        var word = EnglishWords[_random.Next(0, EnglishWords.Length)];
        return word.Contains('\r') ? word.Remove(word.Length - 1, 1) : word;
    }
}