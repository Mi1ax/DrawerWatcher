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
    private const float _colorSize = 16f;

    private readonly Rectangle _bounds;
    
    private readonly Color[] _colors;
    private int _currentColor;

    private readonly float[] _brushesSize;
    private int _currentBrush;

    private readonly Texture _bombTexture;

    public ToolPanel(Rectangle bounds)
    {
        _bounds = bounds;

        _bombTexture = _rl.LoadTexture("Assets/bomb.png");
        _bombTexture.width = 64;
        _bombTexture.height = 64;

        _colors = new []
        {
            (Color)ColorTranslator.FromHtml("#414042"),
            ColorTranslator.FromHtml("#ff675f"),
            ColorTranslator.FromHtml("#a97c50"),
            ColorTranslator.FromHtml("#ff8939"),
            ColorTranslator.FromHtml("#f9ed32"),
            ColorTranslator.FromHtml("#64bf28"),
            ColorTranslator.FromHtml("#30d8d8"),
            ColorTranslator.FromHtml("#27aae1"),
            ColorTranslator.FromHtml("#6d51f4"),
            ColorTranslator.FromHtml("#ee8aff")
        };

        _brushesSize = new []
        {
            8f, 12f, 24f
        };
    }

    public void OnUpdate()
    {
        Renderer.DrawRectangle(_bounds.Size, _bounds.Position, (Color)_rl.Fade(Color.GRAY, 0.4f));

        var endBrushPos = DrawBrushes(_bounds.Position + new Vector2(0, _colorSize + 55));
        var endColorsPos = DrawColors(endBrushPos + new Vector2(55, 0));
        
        DrawTool(_bombTexture, new Vector2(
            endColorsPos.X + 55, 
            _bounds.Position.Y + _colorSize + 15
        ));
    }

    public Vector2 DrawBrushes(Vector2 position)
    {
        for (var brushSizeIndex = 0; brushSizeIndex < _brushesSize.Length; brushSizeIndex++)
        {
            position.X += 30 + _brushesSize[brushSizeIndex] / 2 * brushSizeIndex;

            if (_rl.CheckCollisionPointCircle(
                    Input.GetMousePosition(), 
                    position, _brushesSize[brushSizeIndex]))
            {
                _rl.DrawCircleV(position, _brushesSize[brushSizeIndex], 
                    _rl.Fade(_currentBrush == brushSizeIndex ? Color.BLACK : Color.GRAY, 0.8f));

                if (Input.IsMouseButtonPressed(MouseButton.LEFT))
                {
                    _currentBrush = brushSizeIndex;
                    if (Player.ApplicationOwner != null)
                        Player.ApplicationOwner.CurrentBrush.Thickness = _brushesSize[_currentBrush];
                }
            } else
                _rl.DrawCircleV(position, _brushesSize[brushSizeIndex], 
                    _currentBrush == brushSizeIndex ? Color.BLACK : Color.GRAY);
        }

        return position;
    }
    
    public Vector2 DrawColors(Vector2 position)
    {
        for (var colorIndex = 0; colorIndex < _colors.Length; colorIndex++)
        {
            if (_colors[colorIndex].A == 0) continue;
            
            // ReSharper disable once PossibleLossOfFraction
            position.X += _colorSize + 25;

            if (_rl.CheckCollisionPointCircle(
                    Input.GetMousePosition(),
                    position, _colorSize) || _currentColor == colorIndex)
            {
                _rl.DrawCircle((int)position.X, (int)position.Y, _colorSize + 3f, _rl.GOLD);
                Renderer.DrawCircle(position, _colorSize, _colors[colorIndex]);

                if (!Input.IsMouseButtonPressed(MouseButton.LEFT)) continue;
                
                if (Player.ApplicationOwner != null)
                    Player.ApplicationOwner.CurrentBrush.Color = _colors[colorIndex];
                _currentColor = colorIndex;
            }
            else
                Renderer.DrawCircle(position, _colorSize, _colors[colorIndex]);
        }

        return position;
    }

    public static void DrawTool(Texture texture, Vector2 position)
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

    public void Dispose()
    {
        _rl.UnloadTexture(_bombTexture);
        GC.SuppressFinalize(this);
    }
}