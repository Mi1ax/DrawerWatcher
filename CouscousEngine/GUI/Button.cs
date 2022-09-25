using System.Numerics;
using CouscousEngine.Editor;
using CouscousEngine.Utils;

namespace CouscousEngine.GUI;

public class Button : Visual
{
    [Inspectable] public string Text { get; set; }

    private Action? _onClick;
    
    public Button()
    {
        Text = string.Empty;
        _onClick = null;
    }
    
    public Button(string text)
    {
        Text = text;
        _onClick = null;
    }
    
    public Button(string text, Size size, Vector2 position, Action onClick) 
        : base(size, position)
    {
        Text = text;
        _onClick = onClick;
    }

    public void SetOnClick(Action onClick) => _onClick = onClick;

    public override void Update()
    {
        if (_gui.GuiButton(new Raylib_CsLo.Rectangle(Position.X, Position.Y, Size.Width, Size.Height), Text))
            _onClick?.Invoke();
    }
}