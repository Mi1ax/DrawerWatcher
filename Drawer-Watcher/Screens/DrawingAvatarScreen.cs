namespace Drawer_Watcher.Screens;

public class DrawingAvatarScreen : Screen
{
    private Avatar _avatar;

    public DrawingAvatarScreen()
    {
        _avatar = new Avatar();
    }
    
    public override void OnUpdate()
    {
        _avatar.OnUpdate();
    }

    public override void OnImGuiUpdate()
    {
        
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}