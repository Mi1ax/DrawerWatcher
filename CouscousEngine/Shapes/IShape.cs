﻿using System.Numerics;
using CouscousEngine.Utils;

namespace CouscousEngine.Shapes;

public interface IShape : ICloneable
{
    public void Draw();
    public Vector2 GetPosition();
    public Color GetColor();
}