using System.Diagnostics;

namespace Drawer_Watcher.Managers;

public static class GameLogic
{
    public static string[]? EnglishWords;
    
    public static TimeSpan FinishedTime { get; private set; }
    public static TimeSpan Timer => _stopwatch?.Elapsed ?? TimeSpan.Zero;
    public static string CurrentWord = "";
    
    private static Stopwatch? _stopwatch;
    private static Random? _random;

    public static void Initialize()
    {
        EnglishWords = File.ReadAllText("Assets/Words.txt").Split("\n");
        _random = new Random(DateTime.Now.GetHashCode());
    }

    public static void NewGame()
    {
        
    }

    public static void StartRound()
    {
        _stopwatch = Stopwatch.StartNew();
        if (EnglishWords != null && _random != null)
            CurrentWord = EnglishWords[_random.Next(0, EnglishWords.Length)];
    }

    public static void StopRound()
    {
        FinishedTime = _stopwatch!.Elapsed;
        _stopwatch.Stop();
    }
}