using CouscousEngine.GUI;

namespace Drawer_Watcher.Screens;

public class ConnectionScreen : Screen
{
    private readonly InputBox _inputBox;

    public ConnectionScreen()
    {
        _inputBox = new InputBox();
    }
    
    public override void Update()
    {
        _inputBox.Update();
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}