namespace Drawer_Watcher.Screens;

public abstract class Screen : IDisposable
{
    public abstract void Update();

    public abstract void Dispose();
}