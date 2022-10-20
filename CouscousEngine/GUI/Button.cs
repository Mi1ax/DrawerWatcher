using System.Numerics;
using System.Reflection;
using CouscousEngine.Core;
using CouscousEngine.Utils;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;
using MouseButton = CouscousEngine.Core.MouseButton;

namespace CouscousEngine.GUI;

public class Button
{
    private readonly Rectangle _bounds;

    #region Text

    public string Text
    {
        get => _text;
        set
        {
            _textSize = _rl.MeasureTextEx(_font, value, FontSize, 1f);
            _text = value;
        }
    }
    
    private Vector2 _textSize;

    #endregion
    
    public float CornerRadius = 0;
    public Color Color = Color.GRAY;

    #region Font
    
    private readonly Font _font;
    private string _text;
    private int _fontSize = 24;
    
    public int FontSize 
    {
        get => _fontSize;
        set
        {
            _textSize = _rl.MeasureTextEx(_font, Text, value, 1f);
            _fontSize = value;
        }
    }
    public Color FontColor = Color.WHITE;

    #endregion

    #region Border
    
    public float BorderThickness = 0f;
    public Color BorderColor = Color.BLACK;

    #endregion

    public EventHandler? OnButtonClick = null;
    
    public Button(Rectangle bounds, string text = "Button")
    {
        // TODO: Application.GetDefaultFont()
        _font = AssetManager.GetFont("RobotoMono-Regular-24");
        _text = "";
        Text = text;
        BorderThickness = 1f;
        _bounds = bounds;
    }

    public void OnUpdate() 
    {
        if (CornerRadius != 0)
            _rl.DrawRectangleRounded(_bounds, CornerRadius, 16, Color);
        else
            _rl.DrawRectangleRec(_bounds, Color);
        
        if (BorderThickness != 0)
            _rl.DrawRectangleRoundedLines(
                _bounds, CornerRadius, 16, 
                BorderThickness, BorderColor
                );

        DrawText();
        var isHover = _rl.CheckCollisionPointRec(Input.GetMousePosition(), _bounds); 
        OnHover(isHover);
        if (isHover && Input.IsMouseButtonPressed(MouseButton.LEFT))
            OnButtonClick?.Invoke(this, EventArgs.Empty);
    }

    private void DrawText()
    {
        var textPos = new Vector2(
            _bounds.x + _bounds.width / 2 - _textSize.X / 2,
            _bounds.y + _bounds.height / 2 - _textSize.Y / 2
            );
        _rl.DrawTextEx(_font, Text, textPos, FontSize, 1f, FontColor);
    }

    private void OnHover(bool isHover)
    {
        if (isHover)
            Color = (Color)_rl.Fade(Color, 0.8f);
        else
            Color = (Color)_rl.Fade(Color, 1f);
    }
}