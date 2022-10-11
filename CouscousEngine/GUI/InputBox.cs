using System.Numerics;
using CouscousEngine.Core;
using Raylib_CsLo;

using Color = CouscousEngine.Utils.Color;
using KeyboardKey = CouscousEngine.Core.KeyboardKey;
using MouseButton = CouscousEngine.Core.MouseButton;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace CouscousEngine.GUI;

public class InputBox
{
    private const int _maxCharInBox = 12;
    private const int _maxCharInInput = 32;
    
    private readonly Rectangle _bounds;
    private readonly Vector2 _separatorSize;

    private bool _isUsed;
    private bool _allSelected;

    private string _text;
    private string _displayText;

    private float _backspaceDownTime;
    private float _eraseTime;

    // TODO: Move to asset manager
    private readonly Font _font;

    public InputBox(Rectangle bounds)
    {
        _bounds = bounds;
        _bounds.Color = Color.GRAY;

        unsafe
        {
            fixed (int* codepoints = new int[512])
            {
                for (var i = 0; i < 95; i++) 
                    codepoints[i] = 32 + i;   // Basic ASCII characters
                for (var i = 0; i < 255; i++) 
                    codepoints[96 + i] = 0x400 + i;   // Cyrillic characters
                _font = _rl.LoadFontEx("Assets/Fonts/RobotoMono-Regular.ttf", 24, codepoints, 512);
            }
        }
        
        _rl.SetTextureFilter(_font.texture, TextureFilter.TEXTURE_FILTER_POINT);
        
        _text = "";
        _displayText = "";

        _separatorSize = _rl.MeasureTextEx(_rl.GetFontDefault(), "|", 24f, 1f);
    }

    public void TextEntering()
    {
        if (_text.Length <= _maxCharInInput)
            Input.GetAsciiKeyPressed(ref _text);


        if (Input.IsKeyPressed(KeyboardKey.BACKSPACE) && _text.Length != 0)
        {
            if (_allSelected)
            {
                _text = "";
                _allSelected = false;
            } else
                _text = _text[..^1];
        }

        _backspaceDownTime = Input.GetKeyDownTime(KeyboardKey.BACKSPACE);
        if (_backspaceDownTime >= 0.3f)
        {
            _eraseTime += 0.1f * _rl.GetFrameTime();
            if (!(Math.Abs(_eraseTime - 0.01f) < 0.001f)) return;
            
            if (_text.Length != 0)
                _text = _text[..^1];
            _eraseTime = 0;
        }
        else _eraseTime = 0;
    }

    public void DisplayText()
    {
        _displayText = _text.Length > _maxCharInBox 
            ? _text.Substring(_text.Length - _maxCharInBox, _maxCharInBox) 
            : _text;
        
        var textSize = _rl.MeasureTextEx(_font, _displayText, 24f, 1f);
        
        _rl.DrawTextEx(_font, _displayText, 
            new Vector2(
                _bounds.Position.X + _bounds.Size.Width - 12 - textSize.X, 
                _bounds.Position.Y + _separatorSize.Y / 2.5f), 24f, 1f, Color.BLACK);
    }

    public void CheckActivity()
    {
        if (_rl.CheckCollisionPointRec(Input.GetMousePosition(), _bounds))
        {
            _bounds.Color = (Color)_rl.Fade(Color.GRAY, 0.8f);
            _rl.SetMouseCursor(MouseCursor.MOUSE_CURSOR_IBEAM);
            
            if (Input.IsMouseButtonPressed(MouseButton.LEFT))
            {
                _isUsed = true;
            }
        }
        else if (!_rl.CheckCollisionPointRec(Input.GetMousePosition(), _bounds)
                 && Input.IsMouseButtonPressed(MouseButton.LEFT))
        {
            _isUsed = false;
        }
        else
        {
            _rl.SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT);
            _bounds.Color = Color.GRAY;
        }
    }
    
    public void OnUpdate()
    {
        _bounds.Update();
        Renderer.DrawRectangleLines(_bounds, 1f, Color.BLACK);

        CheckActivity();

        
        if (_isUsed)
        {
            _bounds.Color = (Color)_rl.Fade(Color.GRAY, 0.8f);

            _rl.DrawText("|", 
                _bounds.Position.X + _bounds.Size.Width - 10, 
                _bounds.Position.Y + _separatorSize.Y / 2f, 24f, 
                Color.BLACK
                );
        }

        TextEntering();
        DisplayText();

        if (Input.IsKeyPressedWithModifier(KeyboardKey.LEFT_CONTROL, KeyboardKey.A))
            _allSelected = true;
    }
}