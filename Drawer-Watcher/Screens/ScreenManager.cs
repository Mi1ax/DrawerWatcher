namespace Drawer_Watcher.Screens;

public static class ScreenManager
{
    private static Screen? _currentScreen;

    public static void NavigateTo(Screen newScreen)
        => _currentScreen = newScreen;
    
    
    public static void OnUpdate(float deltaTime) => _currentScreen?.OnUpdate(deltaTime);
    public static void OnUpdateImGui() => _currentScreen?.OnImGuiUpdate();
    public static bool OnEvent() => _currentScreen?.OnEvent() ?? false;
}