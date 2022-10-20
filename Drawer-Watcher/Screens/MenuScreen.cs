using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.GUI;
using CouscousEngine.Utils;
using Raylib_CsLo;

namespace Drawer_Watcher.Screens;

public class MenuScreen : Screen
{
    private readonly Button[] _buttons;

    public MenuScreen()
    {
        _buttons = new Button[2];

        var buttonSize = new Size(125, 55);
        var screenCenter = new Vector2(
            Application.Instance.WindowSize.Width / 2f - buttonSize.Width / 2f,
            Application.Instance.WindowSize.Height / 2f);

        _buttons[0] = new Button(new Rectangle(
            screenCenter.X, screenCenter.Y,
            buttonSize.Width, buttonSize.Height))
        {
            Text = "Join Game",
            OnButtonClick = (sender, args) =>
            {
                ScreenManager.NavigateTo(new ConnectionScreen());
            }
        };
        
        _buttons[1] = new Button(new Rectangle(
            screenCenter.X, screenCenter.Y - buttonSize.Height - 10,
            buttonSize.Width, buttonSize.Height))
        {
            Text = "Create Game",
            OnButtonClick = (sender, args) =>
            {
                ScreenManager.NavigateTo(new CreatingGameScreen());
            }
        };
    }

    public override void OnUpdate()
    {
        foreach (var button in _buttons)
            button.OnUpdate();
    }

    public override void OnImGuiUpdate()
    {
        
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}