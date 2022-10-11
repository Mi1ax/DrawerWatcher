using System.Drawing;
using System.Numerics;
using CouscousEngine.Core;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;
using MouseButton = CouscousEngine.Core.MouseButton;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace Drawer_Watcher.Panels;

public class ToolPanel : IDisposable
{
    private const int _colorPerLine = 10;
    private const int _maxColors = 30;
    private readonly float _colorSize = 16f;
    
    private readonly Rectangle _bounds;

    private int _currentColor = 0;
    
    private readonly Color[] _colors;
    private readonly List<float> _brushesSize;

    private readonly Texture _bombTexture;

    public ToolPanel(Rectangle bounds)
    {
        _bounds = bounds;

        _bombTexture = _rl.LoadTexture("Assets/bomb.png");
        _bombTexture.width = 64;
        _bombTexture.height = 64;

        _colors = new Color[_maxColors];
        Array.Fill(_colors, Color.Empty);

        _colors[0] = ColorTranslator.FromHtml("#414042");
        _colors[1] = ColorTranslator.FromHtml("#ff675f");
        _colors[2] = ColorTranslator.FromHtml("#a97c50");
        _colors[3] = ColorTranslator.FromHtml("#ff8939");
        _colors[4] = ColorTranslator.FromHtml("#f9ed32");
        _colors[5] = ColorTranslator.FromHtml("#64bf28");
        _colors[6] = ColorTranslator.FromHtml("#30d8d8");
        _colors[7] = ColorTranslator.FromHtml("#27aae1");
        _colors[8] = ColorTranslator.FromHtml("#6d51f4");
        _colors[9] = ColorTranslator.FromHtml("#ee8aff");

        _brushesSize = new List<float>();
    }

    public void DrawTool(Texture texture, Vector2 position)
    {
        if (_rl.CheckCollisionPointRec(Input.GetMousePosition(),
                new Raylib_CsLo.Rectangle(position.X, position.Y, texture.width, texture.height)))
        {
            _rl.DrawTexture(texture, 
                (int)position.X, 
                (int)position.Y, 
                _rl.Fade(Color.WHITE, 0.5f));

            if (Input.IsMouseButtonPressed(MouseButton.LEFT))
            {
                Player.SendAllClear();
            }
        } else
            _rl.DrawTexture(texture, 
                (int)position.X, 
                (int)position.Y, 
                Color.WHITE);
    }
    
    public void OnUpdate(bool drawLines = false)
    {
        if (drawLines) Renderer.DrawRectangleLines(_bounds, 2f, Color.RED);
        Renderer.DrawRectangle(_bounds.Size, _bounds.Position, Color.GRAY);

        var bombPosition = new Vector2(
            _bounds.Position.X + _colorPerLine * (_colorSize + 35), 
            _bounds.Position.Y + _colorSize + 15
            );

        DrawTool(_bombTexture, bombPosition);

        for (var colorIndex = 0; colorIndex < _colors.Length; colorIndex++)
        {
            if (_colors[colorIndex].A == 0) continue;
            var position = _bounds.Position + new Vector2(_colorSize + 15, _colorSize + 55);
            
            // ReSharper disable once PossibleLossOfFraction
            position.Y += colorIndex / _colorPerLine * (_colorSize + 25);
            position.X += colorIndex % _colorPerLine * (_colorSize + 25);
            
            var rect = new Raylib_CsLo.Rectangle(
                    position.X, position.Y,
                    _colorSize, _colorSize);
            
            if (_rl.CheckCollisionPointCircle(
                    Input.GetMousePosition(),
                    position, _colorSize) || _currentColor == colorIndex)
            {
                _rl.DrawCircle((int)position.X, (int)position.Y, _colorSize + 3f, _rl.GOLD);
                Renderer.DrawCircle(position, _colorSize, (Color)_rl.Fade(_colors[colorIndex], 0.8f));
                
                if (Input.IsMouseButtonPressed(MouseButton.LEFT))
                {
                    if (Player.ApplicationOwner != null)
                        Player.ApplicationOwner.CurrentBrush.Color = _colors[colorIndex];
                    _currentColor = colorIndex;
                }
            } else
                Renderer.DrawCircle(position, _colorSize, _colors[colorIndex]);
        }
    }


    public void Dispose()
    {
        _rl.UnloadTexture(_bombTexture);
        GC.SuppressFinalize(this);
    }
}