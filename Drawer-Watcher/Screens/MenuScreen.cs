using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.GUI;
using CouscousEngine.Utils;

namespace Drawer_Watcher.Screens;

public class MenuScreen : Screen
{
    private readonly Button[] _buttons;

    public MenuScreen()
    {
        _buttons = new Button[2];

        var buttonSize = new Size(125, 55);
        var screenCenter = new Vector2(
            Application.Instance.GetSize().Width / 2f - buttonSize.Width / 2f,
            Application.Instance.GetSize().Height / 2f);
        
        _buttons[0] = new Button(
            "Join Game",
            buttonSize,
            screenCenter, 
            () =>
            {
                ScreenManager.NavigateTo(new ConnectionScreen());
            });
        
        _buttons[1] = new Button(
            "Create Game",
            buttonSize,
            new Vector2(screenCenter.X, screenCenter.Y - buttonSize.Height - 10),
            () =>
            {
                ScreenManager.NavigateTo(new CreatingGameScreen());
            });
    }
    
    public override void OnUpdate()
    {
        foreach (var button in _buttons)
            button.Update();
    }

    public override void OnImGuiUpdate()
    {
        
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}