using System.Timers;
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
    
    public static int MaxConnection = 4;

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

    public static bool IsRoundEnded = false;
    public static bool IsGameStarted = false;

    private static readonly Random _random = new(DateTime.Now.GetHashCode());
    public static List<string> EnglishWords = 
        File.ReadAllText("Assets/Words.txt").Split("\n").ToList();

    public static string GetRandomWord()
    {
        EnglishWords = File.ReadAllText("Assets/Words.txt").Split("\n").ToList();
        var word = EnglishWords[_random.Next(0, EnglishWords.Count)];
        return word.Contains('\r') ? word.Remove(word.Length - 1, 1) : word;
    }

    public static class Timer
    {
        private static TimeSpan _time;
        private static System.Timers.Timer? _timer;
        private static Action? _onTimerEnds;

        public static bool Enable
        {
            get => _timer is { Enabled: true };
            set
            {
                if (_timer != null) 
                    _timer.Enabled = value;
            }
        }

        public static string CurrentTime = "0:00";
        
        public static void Init()
        {
            _timer?.Dispose();
            _timer = new System.Timers.Timer
            {
                Enabled = false,
                Interval = 1000
            };
            _timer.Elapsed += OnTimerElapsed;
        }

        private static void OnTimerElapsed(object? sender, ElapsedEventArgs args)
        {
            _time -= TimeSpan.FromSeconds(1);
            CurrentTime = $"{_time.Minutes:0}:{_time.Seconds:00}";
            MessageHandlers.SendTime(CurrentTime);
            if (_time != TimeSpan.Zero) return;
            _timer!.Enabled = false;
            _onTimerEnds?.Invoke();
        }

        public static void Start(TimeSpan time, Action onTimerEnds)
        {
            _time = time;
            CurrentTime = $"{_time.Minutes:0}:{_time.Seconds:00}";
            _onTimerEnds = onTimerEnds;
            Enable = true;
        }
    }
}