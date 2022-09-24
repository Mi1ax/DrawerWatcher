using CouscousEngine.Shapes;
using _gui = Raylib_CsLo.RayGui;

namespace CouscousEngine.GUI;

public class Button
{
    private string _text;
    private Rectangle _rectangle;
    private Action _onClick;

    public string Text => _text;
    public Rectangle Rectangle => _rectangle;

    public Button(string text, Rectangle rectangle, Action onClick)
    {
        _text = text;
        _rectangle = rectangle;
        _onClick = onClick;
    }

    public void Draw()
    {
        if (_gui.GuiButton(_rectangle, _text))
            _onClick.Invoke();
    }
}