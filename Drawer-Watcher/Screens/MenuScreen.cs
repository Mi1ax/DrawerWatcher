using System.Drawing;
using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.GUI;
using Color = CouscousEngine.Utils.Color;
using Rectangle = Raylib_CsLo.Rectangle;
using Size = CouscousEngine.Utils.Size;

namespace Drawer_Watcher.Screens;

public class MenuScreen : Screen
{
    private readonly Rectangle _frame;
    private readonly Button[] _buttons;

    public MenuScreen()
    {
        // TODO: Temp positions/sizes
        _frame = new Rectangle(215, 70, 850, 550);
        
        _buttons = new Button[2];

        var buttonSize = new Size(150, 45);

        _buttons[0] = new Button(new Rectangle(
            479, 483,
            buttonSize.Width, buttonSize.Height))
        {
            Text = "Host",
            FontSize = 24,
            FontColor = Color.BLACK,

            CornerRadius = 0.65f,
            BorderThickness = 3f,
            BorderColor = Color.BLACK,
            
            Color = ColorTranslator.FromHtml("#15BAFE"),
            OnButtonClick = (sender, args) =>
            {
                ScreenManager.NavigateTo(new CreatingGameScreen());
            }
        };
        
        _buttons[1] = new Button(new Rectangle(
            645, 483,
            buttonSize.Width, buttonSize.Height))
        {
            Text = "Connect",
            FontSize = 24,
            FontColor = Color.BLACK,

            CornerRadius = 0.65f,
            BorderThickness = 3f,
            BorderColor = Color.BLACK,

            Color = ColorTranslator.FromHtml("#FFBF00"),
            OnButtonClick = (sender, args) =>
            {
                ScreenManager.NavigateTo(new ConnectionScreen());
            }
        };
    }

    public override void OnUpdate()
    {
        _rl.DrawRectangleRounded(_frame, 0.1f, 15, Color.WHITE);

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