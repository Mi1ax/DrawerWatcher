using CouscousEngine.Core;

namespace Drawer_Watcher.Screens;

public static class ScreenManager
{
    private static bool _captureEvents = true;
    private static Screen? _currentScreen;

    public static Screen? CurrentScreen => _currentScreen;
    
    public static void NavigateTo(Screen newScreen)
        => _currentScreen = newScreen;

    public static void OnEsc()
    {
        if (Input.IsKeyPressed(KeyboardKey.ESCAPE))
            _captureEvents = !_captureEvents;
    }
    
    public static void OnUpdate(float deltaTime) => _currentScreen?.OnUpdate(deltaTime);
    public static void OnUpdateImGui() => _currentScreen?.OnImGuiUpdate();

    public static bool OnEvent()
    {
        OnEsc();
        if (_captureEvents)
            return _currentScreen?.OnEvent() ?? false;
        return false;
    }
}