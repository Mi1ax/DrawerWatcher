using System.Numerics;
using CouscousEngine.Core;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;
using MouseButton = CouscousEngine.Core.MouseButton;

namespace Drawer_Watcher;

public class Avatar : IDisposable
{
    private readonly RenderTexture _renderTexture;

    private Vector2 _currPos = Vector2.Zero;
    private Vector2 _prevPos = Vector2.Zero;
    
    public Avatar(uint width = 64, uint height = 64)
    {
        _renderTexture = Renderer.LoadRenderTexture(64, 64);
    }

    public void OnUpdate()
    {
        _rl.DrawTexturePro(_renderTexture.texture, 
            new Rectangle(
                0, 0, 
                64, -64
                ), 
            new Rectangle(
                0, 0, 
                640, 640
                ), 
            Vector2.Zero, 0f, Color.WHITE);
        
        _rl.DrawRectangleLines(0, 0, 640, 640, Color.BLACK);
        
        _currPos = Input.GetMousePosition() / 10;
        if (Input.IsMouseButtonPressed(MouseButton.LEFT))
        {
            Renderer.BeginTextureMode(_renderTexture);
            _rl.DrawPixelV(_currPos, Color.BLACK);
            Renderer.EndTextureMode();
        } 
        else if (Input.IsMouseButtonPressed(MouseButton.RIGHT))
        {
            Renderer.BeginTextureMode(_renderTexture);
            _rl.DrawPixelV(_currPos, Color.WHITE);
            Renderer.EndTextureMode();
        }
        if (Input.IsMouseButtonDown(MouseButton.LEFT))
        {
            Renderer.BeginTextureMode(_renderTexture);
            _rl.DrawLineEx(_currPos, _prevPos, 1f, Color.BLACK);
            Renderer.EndTextureMode();
        } 
        else if (Input.IsMouseButtonDown(MouseButton.RIGHT))
        {
            Renderer.BeginTextureMode(_renderTexture);
            _rl.DrawLineEx(_currPos, _prevPos, 1f, Color.WHITE);
            Renderer.EndTextureMode();
        }
        _prevPos = _currPos;
    }

    public Image GetImage()
        => _rl.LoadImageFromTexture(_renderTexture.texture);

    public void Dispose()
    {
        Renderer.UnloadRenderTexture(_renderTexture);
        GC.SuppressFinalize(this);
    }
}