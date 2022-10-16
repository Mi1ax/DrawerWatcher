using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Utils;
using Raylib_CsLo;

using Color = CouscousEngine.Utils.Color;
using KeyboardKey = CouscousEngine.Core.KeyboardKey;
using MouseButton = CouscousEngine.Core.MouseButton;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace CouscousEngine.GUI;

public class InputBox : IDisposable
{
    public Vector2 Position
    {
        get => _bounds.Position;
        set => _bounds.Position = value;
    }

    public Size Size
    {
        get => _bounds.Size;
        set => _bounds.Size = value;
    }

    public string Text
    {
        get => _text;
        set => _text = value;
    }

    public bool IsEnable { get; set; } = true;

    public Font Font => _font;
    
    private readonly int _maxCharInBox;
    private const int _maxCharInInput = 32;
    
    private const float _eraseSpeed = 0.005f;

    private readonly Rectangle _bounds;
    private readonly Vector2 _separatorSize;
    
    private bool _isUsed;
    private bool _allSelected;

    private string _text;
    private string _displayText;

    private float _backspaceDownTime;
    private float _eraseTime;

    private readonly Font _font;

    private Action? OnEnterPressed;

    public InputBox(Rectangle bounds)
    {
        _bounds = bounds;
        _bounds.Color = Color.GRAY;

        var charCount = (int)(_bounds.Size.Width - 27) / 10;
        _maxCharInBox = (int)(_bounds.Size.Width - 27 - charCount) / 10 + 1; 

        _font = AssetManager.GetDefaultFont(24);

        _rl.SetTextureFilter(_font.texture, TextureFilter.TEXTURE_FILTER_POINT);
        
        _text = "";
        _displayText = "";

        _separatorSize = _rl.MeasureTextEx(_rl.GetFontDefault(), "|", 24f, 1f);
    }

    public void SetAction(Action action) => OnEnterPressed = action;

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
            if (!(Math.Abs(_eraseTime - _eraseSpeed) < 0.001f)) return;
            
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
        
        _rl.DrawTextEx(_font, _displayText, 
            new Vector2(
                _bounds.Position.X + 12, 
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

        if (!_isUsed) return;
        
        _bounds.Color = (Color)_rl.Fade(Color.GRAY, 0.8f);

        var textSize = _rl.MeasureTextEx(_font, _displayText, 24f, 1f);

        _rl.DrawText("|", 
            _bounds.Position.X + textSize.X + 15, 
            _bounds.Position.Y + _separatorSize.Y / 2f, 24f, 
            Color.BLACK
        );
        
        if (Input.IsKeyPressed(KeyboardKey.ENTER))
            OnEnterPressed?.Invoke();
    }
    
    public void OnUpdate()
    {
        _bounds.Update();
        Renderer.DrawRectangleLines(_bounds, 1f, Color.BLACK);

        if (!IsEnable) return;
        
        CheckActivity();
        TextEntering();
        DisplayText();

        if (Input.IsKeyPressedWithModifier(KeyboardKey.LEFT_CONTROL, KeyboardKey.A))
            _allSelected = true;
    }

    public void Dispose()
    {
        _rl.SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT);
        GC.SuppressFinalize(this);
    }
}