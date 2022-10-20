using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Utils;
using Raylib_CsLo;

using Color = CouscousEngine.Utils.Color;
using KeyboardKey = CouscousEngine.Core.KeyboardKey;
using MouseButton = CouscousEngine.Core.MouseButton;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace CouscousEngine.GUI;

public class Entry
{
    public Vector2 Position 
    {
        get => new (_bounds.x, _bounds.y);
        set
        {
            _bounds.x = value.X;
            _bounds.y = value.Y;
        }
    }

    public Size Size 
    {
        get => new (_bounds.width, _bounds.height);
        set
        {
            _bounds.width = value.Width;
            _bounds.height = value.Height;
        }
    }

    #region Text

    private readonly Font _font;
    private string _text;
    private string _displayText;
    private int _fontSize = 24;
    
    private Vector2 _textSize;
    private readonly Vector2 _separatorSize;

    public Font Font => _font;
    public Color FontColor = Color.BLACK;

    public string Text
    {
        get => _text;
        set
        {
            _textSize = _rl.MeasureTextEx(_font, value, TextSize, 1f);
            _text = value;
        }
    }
    
    private int TextSize
    {
        get => _fontSize;
        set
        {
            _textSize = _rl.MeasureTextEx(_font, Text, value, 1f);
            _fontSize = value;
        }
    }

    #endregion

    #region Border

    public float BordeThickness = 1f;
    public Color BorderColor = Color.BLACK;

    #endregion
    
    #region Entry Fields

    private readonly int _maxCharInBox;
    private const int _maxCharInInput = 32;
    
    private const float _eraseSpeed = 0.005f;
    
    private bool _isUsed;
    private bool _allSelected;
    
    private float _backspaceDownTime;
    private float _eraseTime;

    #endregion
    
    public bool IsEnable { get; set; } = true;
    public Color Color { get; set; }
    public EventHandler? OnEnterPressed = null;
    
    private Raylib_CsLo.Rectangle _bounds;
    
    public float CornerRadius = 0;

    public Entry(Raylib_CsLo.Rectangle bounds)
    {
        // TODO: Application.GetDefaultFont()
        _font = AssetManager.GetFont("RobotoMono-Regular-24");
        _text = "";
        _displayText = "";
        _bounds = bounds;
        Text = _text;
        
        Color = Color.WHITE;
        FontColor = Color.BLACK;
        
        var charCount = (int)(_bounds.width - 27) / 10;
        _maxCharInBox = (int)(_bounds.width - 27 - charCount) / 10 + 1; 

        _font = AssetManager.GetDefaultFont(24);

        _rl.SetTextureFilter(_font.texture, TextureFilter.TEXTURE_FILTER_POINT);

        _separatorSize = _rl.MeasureTextEx(_rl.GetFontDefault(), "|", 24f, 1f);
    }

    public void OnUpdate() 
    {
        if (CornerRadius != 0)
        {
            _rl.DrawRectangleRounded(_bounds, CornerRadius, 15, Color);
            _rl.DrawRectangleRoundedLines(
                _bounds, CornerRadius, 
                15, BordeThickness, BorderColor
            );
        }
        else
        {
            _rl.DrawRectangleRec(_bounds, Color);
            _rl.DrawRectangleLinesEx(_bounds, BordeThickness, BorderColor);
        }

        if (!IsEnable) return;
        
        CheckActivity();
        TextEntering();
        DisplayText();

        if (Input.IsKeyPressedWithModifier(KeyboardKey.LEFT_CONTROL, KeyboardKey.A))
            _allSelected = true;
    }
    
    private void TextEntering() 
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

    private void DisplayText() 
    {
        _displayText = _text.Length > _maxCharInBox
            ? _text.Substring(_text.Length - _maxCharInBox, _maxCharInBox) 
            : _text;
        
        _rl.DrawTextEx(_font, _displayText, 
            new Vector2(
                _bounds.X + 12, 
                _bounds.Y + _separatorSize.Y / 2.5f), 24f, 1f, FontColor);
    }

    private void CheckActivity() 
    {
        if (_rl.CheckCollisionPointRec(Input.GetMousePosition(), _bounds))
        {
            Color = (Color)_rl.Fade(Color, 0.8f);

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
            Color = (Color)_rl.Fade(Color, 1f);
        }

        if (!_isUsed) return;
        
        Color = (Color)_rl.Fade(Color, 0.8f);

        var textSize = _rl.MeasureTextEx(_font, _displayText, 24f, 1f);

        _rl.DrawText("|", 
            _bounds.X + textSize.X + 15, 
            _bounds.Y + _separatorSize.Y / 2f, 24f, 
            FontColor
        );
        
        if (Input.IsKeyPressed(KeyboardKey.ENTER))
            OnEnterPressed?.Invoke(this, EventArgs.Empty);
    }
}