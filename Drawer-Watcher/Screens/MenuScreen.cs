using System.Drawing;
using System.Numerics;
using CouscousEngine.GUI;
using CouscousEngine.Shapes;
using Color = CouscousEngine.Utils.Color;
using Rectangle = Raylib_CsLo.Rectangle;
using Size = CouscousEngine.Utils.Size;

namespace Drawer_Watcher.Screens;

public class MenuScreen : Screen
{
    private readonly Rectangle _frame;
    
    private readonly Entry[] _entries;
    private readonly Button[] _buttons;

    public MenuScreen()
    {
        // TODO: Temp positions/sizes
        _frame = new Rectangle(215, 85, 850, 550);

        _buttons = new Button[2];
        ButtonsInit();

        _entries = new Entry[2];
        EntriesInit();
    }

    private void ButtonsInit() 
    {
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
                ScreenManager.NavigateTo(new CreatingGameScreen(_entries[0].Text));
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
                ScreenManager.NavigateTo(new ConnectionScreen(_entries[0].Text));
            }
        };
    }

    private void EntriesInit() 
    {
        var entrySize = new Size(180, 45);

        _entries[0] = new Entry(new Rectangle(
            621, 347, 
            entrySize.Width, entrySize.Height)
        )
        {
            Placeholder = "Player 1",
            
            BordeThickness = 2f,
            BorderColor = Color.BLACK,
            CornerRadius = 0.65f,
            
            Label = "Nickname",
            LabelFontSize = 32
        };
        
        _entries[1] = new Entry(new Rectangle(
            621, 409, 
            entrySize.Width, entrySize.Height)
        )
        {
            Placeholder = "English",
            
            BordeThickness = 2f,
            BorderColor = Color.BLACK,
            CornerRadius = 0.65f,
            
            Label = "Language",
            LabelFontSize = 32
        };
    }
    
    public override void OnUpdate(float deltaTime)
    {
        _rl.DrawRectangleRounded(_frame, 0.1f, 15, Color.WHITE);

        foreach (var button in _buttons)
            button.OnUpdate(deltaTime);

        foreach (var entry in _entries)
            entry.OnUpdate(deltaTime);
        
        _rl.DrawCircleV(new Vector2(640, 237), 75, Color.GRAY);
    }

    public override bool OnEvent()
    {
        return _entries.Any(entry => entry.OnEvent()) || 
               _buttons.Any(entry => entry.OnEvent());
    }
}