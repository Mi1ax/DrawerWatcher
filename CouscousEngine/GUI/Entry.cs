using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Utils;
using Raylib_CsLo;

using Color = CouscousEngine.Utils.Color;
using KeyboardKey = CouscousEngine.Core.KeyboardKey;
using MouseButton = CouscousEngine.Core.MouseButton;

namespace CouscousEngine.GUI;

public class Entry : Visual
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
    public Color FontColor;

    public string Text 
    {
        get => _text;
        set
        {
            _textSize = _rl.MeasureTextEx(_font, value, TextSize, 1f);
            _text = value;
        }
    }

    public string Placeholder;
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

    #region Label

    public string Label
    {
        get => _label;
        set
        {
            _labelSize = _rl.MeasureTextEx(LabelFont, value, LabelFontSize, 1f);
            _label = value;
        }
    }

    public int LabelFontSize = 24;
    public Font LabelFont;
    
    private string _label = "";
    private Vector2 _labelSize;

    #endregion
    
    public bool IsEnable { get; set; } = true;
    public Color Color { get; set; }
    public EventHandler? OnEnterPressed = null;
    
    private Raylib_CsLo.Rectangle _bounds;
    
    public float CornerRadius = 0;

    public Entry(Rectangle bounds) 
    {
        // TODO: Application.GetDefaultFont()
        _font = AssetManager.GetFont("RobotoMono-Regular-24");
        _text = "";
        _displayText = "";
        _isUsed = false;
        _bounds = bounds;
        
        Text = _text;
        Placeholder = "";
        Color = Color.WHITE;
        FontColor = Color.BLACK;
        
        var charCount = (int)(_bounds.width - 27) / 10;
        _maxCharInBox = (int)(_bounds.width - 27 - charCount) / 10 + 1; 

        _font = AssetManager.GetDefaultFont(24);
        LabelFont = AssetManager.GetDefaultFont(32);

        _rl.SetTextureFilter(_font.texture, TextureFilter.TEXTURE_FILTER_POINT);

        _separatorSize = _rl.MeasureTextEx(_rl.GetFontDefault(), "|", 24f, 1f);
    }
    
    private void TextEntering() 
    {
        if (!_isUsed) return;
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
        if (Label != "")
        {
            _rl.DrawTextEx(LabelFont, _label,
                new Vector2(
                    _bounds.x - _labelSize.X - 45,
                    _bounds.y + _labelSize.Y / 4
                    ), LabelFontSize, 1f, Color.BLACK);
        }
        
        if (_text != "")
        {
            _displayText = _text.Length > _maxCharInBox
                ? _text.Substring(_text.Length - _maxCharInBox, _maxCharInBox) 
                : _text;
        
            _rl.DrawTextEx(_font, _displayText, 
                new Vector2(
                    _bounds.X + 12, 
                    _bounds.Y + _separatorSize.Y / 2.5f), 24f, 1f, FontColor);
        }
        else if (!_isUsed)
        {
            _rl.DrawTextEx(_font, Placeholder, 
                new Vector2(
                    _bounds.X + 12, 
                    _bounds.Y + _separatorSize.Y / 2.5f),
                24f, 1f, Color.GRAY);
        }

        if (!_isUsed) return;
        
        var textSize = _rl.MeasureTextEx(_font, _displayText, 24f, 1f);
        _rl.DrawText("|", 
            _bounds.X + (_text == "" ? 0 : textSize.X) + 15, 
            _bounds.Y + _separatorSize.Y / 2f, 24f, 
            FontColor
        );
    }

    private bool CheckActivity() 
    {
        if (_rl.CheckCollisionPointRec(Input.GetMousePosition(), _bounds))
        {
            Color = (Color)_rl.Fade(Color, 0.8f);

            if (Input.IsMouseButtonPressed(MouseButton.LEFT))
            {
                _isUsed = true;
                return true;
            }
        }
        else if (!_rl.CheckCollisionPointRec(Input.GetMousePosition(), _bounds)
                 && Input.IsMouseButtonPressed(MouseButton.LEFT))
        {
            _isUsed = false;
            return false;
        }
        else
        {
            Color = (Color)_rl.Fade(Color, 1f);
        }

        if (!_isUsed) return false;
        
        Color = (Color)_rl.Fade(Color, 0.8f);

        if (Input.IsKeyPressed(KeyboardKey.ENTER))
            OnEnterPressed?.Invoke(this, EventArgs.Empty);
        return false;
    }

    public override void OnUpdate(float deltaTime)
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

        DisplayText();
        TextEntering();
    }

    public override bool OnEvent()
    {
        if (!IsEnable) return false;

        var activity = CheckActivity();

        if (_isUsed && Input.IsKeyPressedWithModifier(KeyboardKey.LEFT_CONTROL, KeyboardKey.A))
            _allSelected = true;
        
        return activity;
    }
}