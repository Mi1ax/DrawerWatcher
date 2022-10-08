namespace Drawer_Watcher.Screens;

public abstract class Screen : IDisposable
{
    public abstract void OnUpdate();
    public abstract void OnImGuiUpdate();

    public abstract void Dispose();
}