﻿using System.Numerics;
using CouscousEngine.Utils;

namespace CouscousEngine.GUI;

public class Button : Visual
{
    private string _text;

    private Action? _onClick;

    public Button(string text)
    {
        _text = text;
        _onClick = null;
    }
    
    public Button(string text, Size size, Vector2 position, Action onClick) 
        : base(size, position)
    {
        _text = text;
        _onClick = onClick;
    }

    public void SetText(string value) => _text = value;

    public override void Update()
    {
        if (_gui.GuiButton(new Raylib_CsLo.Rectangle(Position.X, Position.Y, Size.Width, Size.Height), _text))
            _onClick?.Invoke();
    }
}