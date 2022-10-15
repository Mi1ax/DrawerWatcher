using System.Diagnostics;
using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Utils;

namespace Drawer_Watcher.Managers;

public static class GameLogic
{
    public static string[]? EnglishWords;
    
    public static TimeSpan FinishedTime { get; private set; }
    public static TimeSpan Timer => _stopwatch?.Elapsed ?? TimeSpan.Zero;
    public static string CurrentWord = "";
    public static ushort Winner = 0;

    private static Stopwatch? _stopwatch;
    private static Random? _random;

    public static void Initialize()
    {
        EnglishWords = File.ReadAllText("Assets/Words.txt").Split("\n");
        _random = new Random(DateTime.Now.GetHashCode());
    }
    
    public static void StartRound()
    {
        _stopwatch = Stopwatch.StartNew();
        if (Player.ApplicationOwner is not {IsDrawer: true}) return;
        if (EnglishWords == null || _random == null) return;
        
        var word = EnglishWords[_random.Next(0, EnglishWords.Length)];
        CurrentWord = word.Contains('\r') ? word.Remove(word.Length - 1, 1) : word;
    }

    public static void StopRound()
    {
        FinishedTime = _stopwatch!.Elapsed;
        _stopwatch.Stop();
    }
}