using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Shapes;
using CouscousEngine.Utils;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Panels;
using ImGuiNET;

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
        GameLogic.StartRound();
        if (Player.ApplicationOwner is {IsDrawer: true})
            _chatPanel.DisableInput = true;
    }

    public override void OnUpdate()
    {
        if (!NetworkManager.IsClientConnected || NetworkManager.Players.Count == 0) return;
            
        foreach (var player in NetworkManager.Players.Values)
            player.Update();
            
        Renderer.DrawTexture(GameData.Painting!.Value, _drawingPanel.Position, Color.WHITE);
        Renderer.DrawRectangleLines(_drawingPanel, 1f, Color.BLACK);

        _chatPanel.OnUpdate();
        
        if (Player.ApplicationOwner is {IsDrawer: true})
            _toolPanel.OnUpdate();

        if (Player.ApplicationOwner is {IsDrawer: true})
        {
            var textSize = _rl.MeasureTextEx(
                AssetManager.GetFont("RobotoMono-Regular"), 
                GameLogic.CurrentWord, 48f, 1f
            );
        
            _rl.DrawTextEx(AssetManager.GetFont("RobotoMono-Regular"), 
                GameLogic.CurrentWord, 
                new Vector2(
                    _drawingPanel.Size.Width / 2 - textSize.X,
                    25
                ), 
                48f, 1f, Color.BLACK);
        }
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