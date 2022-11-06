using System.Numerics;
using CouscousEngine.Core;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Panels;
using ImGuiNET;

namespace Drawer_Watcher.Screens;

public class GameScreen : Screen
{
    private bool _viewportFocused;
    private bool _viewportHovered;
    private Vector2 _viewportSize = Vector2.Zero;

    private readonly StatPanel _statPanel = new();
    private readonly ChatPanel _chatPanel = new();
    private readonly ToolPanel _toolPanel = new();
    
    public static Vector2 CursorOffset = Vector2.Zero;

    public GameScreen()
    {
        GameData.Painting = Renderer.LoadRenderTexture(1, 1);

        if (Player.ApplicationOwner is { IsDrawer: true })
        {
            NewWord();
            _chatPanel.DisableInput = true;
        }
    }

    private static void NewWord()
    {
        MessageHandlers.GetNewWord();
        MessageHandlers.ClearPainting();
        GameManager.Guesser = 0;
    }

    public override void OnUpdate(float deltaTime)
    {
        if (!NetworkManager.IsClientConnected || NetworkManager.Players.Count == 0) return;

        if (_viewportSize.X > 0.0f && _viewportSize.Y > 0.0f &&
            ((int)_viewportSize.X != GameData.Painting!.Value.texture.width || 
             (int)_viewportSize.Y != GameData.Painting.Value.texture.height))
        {
            _rl.UnloadRenderTexture(GameData.Painting.Value);
            GameData.Painting = Renderer.LoadRenderTexture((int)_viewportSize.X, (int)_viewportSize.Y);
            MessageHandlers.ClearPainting();
        }
        
        foreach (var player in NetworkManager.Players.Values)
            player.Update(_viewportHovered && _viewportFocused);
    }

    public override void OnImGuiUpdate()
    {
        var window_flags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
        
        var viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        ImGui.SetNextWindowViewport(viewport.ID);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        window_flags |= ImGuiWindowFlags.NoTitleBar | 
                        ImGuiWindowFlags.NoCollapse | 
                        ImGuiWindowFlags.NoResize | 
                        ImGuiWindowFlags.NoMove;
        window_flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | 
                        ImGuiWindowFlags.NoNavFocus;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

        ImGui.Begin("MainDockspace", window_flags);
        {
            ImGui.PopStyleVar();
            
            ImGui.PopStyleVar(2);

            var io = ImGui.GetIO();

            if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.DockingEnable))
            {
                var dockspaceID = ImGui.GetID("MyDockSpace");
                // TODO: Disable resizing
                ImGui.DockSpace(dockspaceID, Vector2.Zero, ImGuiDockNodeFlags.NoResize);
            }

            var flags = ImGuiWindowFlags.NoNav | 
                        ImGuiWindowFlags.NoResize | 
                        ImGuiWindowFlags.NoCollapse |
                        ImGuiWindowFlags.NoTitleBar |
                        ImGuiWindowFlags.NoMove;
            _statPanel.OnImGuiUpdate(flags);
            _chatPanel.OnImGuiUpdate(flags);
            _toolPanel.OnImGuiUpdate(flags);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.Begin("Viewport", ImGuiWindowFlags.NoTitleBar);
            {
                _viewportFocused = ImGui.IsWindowFocused();
                _viewportHovered = ImGui.IsWindowHovered();
                _viewportSize = ImGui.GetContentRegionAvail();
                CursorOffset = ImGui.GetCursorScreenPos();
                
                ImGui.Image(new IntPtr(GameData.Painting!.Value.texture.id), 
                    _viewportSize, new Vector2(0, 1), new Vector2(1, 0));
            }
            ImGui.End();
            ImGui.PopStyleVar();
        }
        ImGui.End();
    }
}