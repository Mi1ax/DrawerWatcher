using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Utils;
using ImGuiNET;
using Raylib_CsLo;
using Color = CouscousEngine.Utils.Color;
using MouseButton = CouscousEngine.Core.MouseButton;

namespace Sandbox;

public class SandboxLayer : Layer
{
    private bool _viewportFocused;
    private bool _viewportHovered;
    private Vector2 _viewportSize = Vector2.Zero;
    
    private Vector3 _brushColor = new (1f, 1f, 1f);
    private Vector3 _clearColor = new (1f, 1f, 1f);

    private Vector2 _cursorOffset;
    private Vector2 _currMousePos = Vector2.Zero;
    private Vector2 _prevMousePos = Vector2.Zero;

    private RenderTexture _renderTexture;

    private List<string> _chatMessages = new();

    public SandboxLayer() 
        : base("SandboxLayer")
    {
        _renderTexture = Renderer.LoadRenderTexture(1, 1);
    }

    public override void OnUpdate(float deltaTime)
    {
        if (_viewportSize.X > 0.0f && _viewportSize.Y > 0.0f &&
            ((int)_viewportSize.X != _renderTexture.texture.width || 
             (int)_viewportSize.Y != _renderTexture.texture.height))
        {
            _rl.UnloadRenderTexture(_renderTexture);
            _renderTexture = Renderer.LoadRenderTexture((int)_viewportSize.X, (int)_viewportSize.Y);
        }

        _currMousePos = Input.GetMousePosition() - _cursorOffset;
        if (_viewportHovered && _viewportFocused)
        {
            Renderer.BeginTextureMode(_renderTexture);
            if (Input.IsMouseButtonDown(MouseButton.LEFT))
            {
                Renderer.MouseDrawing(_currMousePos, _prevMousePos, 8f, (Color)_brushColor);
            }
            Renderer.EndTextureMode();
        }
        _prevMousePos = _currMousePos;
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
                var dockspace_id = ImGui.GetID("MyDockSpace");
                ImGui.DockSpace(dockspace_id, Vector2.Zero, ImGuiDockNodeFlags.None);
            }

            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Exit"))
                        Application.Instance.Close();
                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar();
            }

            // ImGui Drawing Here

            ImGui.Begin("Chat");
            {
                var footerHeightToReserve = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
                if (ImGui.BeginChild("ScrollingRegion", new Vector2(0, -footerHeightToReserve), 
                        false, ImGuiWindowFlags.HorizontalScrollbar))
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 1)); // Tighten spacing
                    foreach (var item in _chatMessages)
                    {
                        ImGui.Text(item);
                    }

                    if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                        ImGui.SetScrollHereY(1.0f);

                    ImGui.PopStyleVar();
                }
                ImGui.EndChild();
                ImGui.Separator();
                
                var text = "";
                const ImGuiInputTextFlags inputConfig = ImGuiInputTextFlags.EnterReturnsTrue;
                if (ImGui.InputText("", ref text, 128, inputConfig))
                {
                    _chatMessages.Add(text);
                }
            }
            ImGui.End();
            
            ImGui.Begin("Settings");
            {
                ImGui.ColorEdit3("Clear Color", ref _clearColor);
                if (ImGui.Button("Clear Canvas"))
                {
                    Renderer.BeginTextureMode(_renderTexture);
                    Renderer.ClearBackground((Color)_clearColor);
                    Renderer.EndTextureMode();
                }
                ImGui.ColorEdit3("Brush Color", ref _brushColor);
            }
            ImGui.End();
            
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.Begin("Viewport");
            {
                _viewportFocused = ImGui.IsWindowFocused();
                _viewportHovered = ImGui.IsWindowHovered();
                _viewportSize = ImGui.GetContentRegionAvail();
                _cursorOffset = ImGui.GetCursorScreenPos();
                
                ImGui.Image(new IntPtr(_renderTexture.texture.id), 
                    _viewportSize, new Vector2(0, 1), new Vector2(1, 0));
            }
            ImGui.End();
            ImGui.PopStyleVar();
        }
        ImGui.End();
    }

    public override bool OnEvent()
    {
        return false;
    }
}