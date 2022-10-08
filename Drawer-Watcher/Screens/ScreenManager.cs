namespace Drawer_Watcher.Screens;

public static class ScreenManager
{
    private static Screen? _currentScreen;

    public static void NavigateTo(Screen newScreen)
    {
        _currentScreen?.Dispose();
        _currentScreen = newScreen;
    }

    public static void UpdateImGui() => _currentScreen?.OnImGuiUpdate();

    public static void Update() => _currentScreen?.OnUpdate();
}