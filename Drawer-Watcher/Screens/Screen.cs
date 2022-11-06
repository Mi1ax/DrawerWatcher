namespace Drawer_Watcher.Screens;

public abstract class Screen
{
    public abstract void OnUpdate(float deltaTime);
    public virtual void OnImGuiUpdate() {}
    public virtual bool OnEvent() => false;
}