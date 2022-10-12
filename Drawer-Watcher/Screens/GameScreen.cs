using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Shapes;
using CouscousEngine.Utils;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Panels;

namespace Drawer_Watcher.Screens;

public class GameScreen : Screen
{
    private readonly Rectangle _drawingPanel;
    private readonly ChatPanel _chatPanel;
    private readonly ToolPanel _toolPanel;

    public GameScreen()
    {
        _chatPanel = new ChatPanel(new Rectangle(new Size(350, 720), new Vector2(930, 0))
        {
            Color = new Color(193, 193, 193)
        });
        
        _drawingPanel = new Rectangle(new Size(930, 720 - 144), Vector2.Zero);

        _toolPanel = new ToolPanel(new Rectangle(new Size(930, 144), new Vector2(0, 720 - 144))
        {
            Color = Color.GRAY
        });
        
        GameData.Painting = Renderer.LoadRenderTexture((int)_drawingPanel.Size.Width, (int)_drawingPanel.Size.Height);
    }

    public override void OnUpdate()
    {
        if (!NetworkManager.IsConnectedToServer || GameManager.Players.Count == 0) return;
            
        foreach (var player in GameManager.Players.Values)
            player.Update();
            
        Renderer.DrawTexture(GameData.Painting!.Value, _drawingPanel.Position, Color.WHITE);
        Renderer.DrawRectangleLines(_drawingPanel, 1f, Color.BLACK);

        _chatPanel.OnUpdate();
        _toolPanel.OnUpdate();
    }

    public override void OnImGuiUpdate()
    {
        
    }

    public override void Dispose()
    {
        _toolPanel.Dispose();
        _chatPanel.Dispose();
        GC.SuppressFinalize(this);
    }
}