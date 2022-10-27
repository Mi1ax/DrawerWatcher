using System.Numerics;
using CouscousEngine.Core;

namespace Drawer_Watcher.Screens;

public abstract class Screen : IDisposable
{
    public bool IsMenuOpen;
    public bool DisableMenu;
    
    public abstract void OnUpdate();
    
    public abstract void OnImGuiUpdate();

    public abstract void Dispose();

    public void OnMenuUpdate()
    {
        if (!IsMenuOpen) return;
        _rl.DrawRectangleV(
            Vector2.Zero, 
            Application.Instance.WindowSize, 
            _rl.Fade(_rl.WHITE, 0.8f)
        );
    }

    public void OnEscPressed()
    {
        if (!DisableMenu)
            IsMenuOpen = !IsMenuOpen;
    }
}