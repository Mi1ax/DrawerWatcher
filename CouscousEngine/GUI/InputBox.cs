using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Utils;
using Raylib_CsLo;
using _gui = Raylib_CsLo.RayGui;
using _rl = Raylib_CsLo.Raylib;
using Color = CouscousEngine.Utils.Color;
using KeyboardKey = CouscousEngine.Core.KeyboardKey;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace CouscousEngine.GUI;

public class InputBox
{
    private const int FontSize = 28;
    
    private readonly Rectangle _rectangle;
    private string _text;

    private bool _isMouseOn;
    private int _frameCount;
    
    public InputBox()
    {
        _text = "";
        var measureText = _rl.MeasureTextEx(_rl.GetFontDefault(), "123", FontSize, 2f);
        _rectangle = new Rectangle(new Size(125, measureText.Y), Vector2.Zero);
    }

    public void Update()
    {
        _isMouseOn = _rl.CheckCollisionPointRec(Input.GetMousePosition(), _rectangle);

        if (_isMouseOn)
        {
            _rl.SetMouseCursor(MouseCursor.MOUSE_CURSOR_IBEAM);
            var key = _rl.GetCharPressed();
            
            while (key > 0)
            {
                if (key is >= 32 and <= 125 && _text.Length < 16)
                {
                    _text += (char)key;
                }

                key = _rl.GetCharPressed();
            }

            if (Input.IsKeyPressed(KeyboardKey.BACKSPACE) && _text.Length > 0)
            {
                _text = _text[..^1];
            }
        }
        else
        {
            _rl.SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT);
        }
        
        if (_isMouseOn) _frameCount++;
        else _frameCount = 0;
        
        _rl.DrawRectangleRec(_rectangle, _rl.LIGHTGRAY);
        _rl.DrawRectangleLines(
            (int)_rectangle.Position.X,
            (int)_rectangle.Position.Y,
            (int)_rectangle.Size.Width, (int)_rectangle.Size.Height, _isMouseOn ? _rl.RED : _rl.DARKGRAY);

        _rl.DrawText(_text, 
            (int)_rectangle.Position.X + 5, (int)_rectangle.Position.Y, 
            FontSize, _rl.MAROON);

        if (!_isMouseOn) return;
        if (_text.Length >= 32) return;
        
        if (_frameCount / 20 % 2 == 0) 
            _rl.DrawText("_", 
                (int)_rectangle.Position.X + 8 + _rl.MeasureText(_text, FontSize), 
                (int)_rectangle.Position.Y, FontSize, Color.RED);
    }
}