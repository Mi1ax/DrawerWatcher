using System.Numerics;
using CouscousEngine.Core;
using ImGuiNET;

namespace Sandbox;

public class SandboxLayer : Layer
{
    private bool _viewportFocused;
    private bool _viewportHovered;
    private Vector2 _viewportSize = Vector2.Zero;
    
    public SandboxLayer() 
        : base("SandboxLayer")
    {
        
    }

    public override void OnUpdate(float deltaTime)
    {
        
    }

    public override void OnImGuiUpdate()
    {
        var dockspaceOpen = true;
        var opt_fullscreen = true;
        var opt_padding = false;
        var dockspace_flags = ImGuiDockNodeFlags.None;

        var window_flags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
        if (opt_fullscreen)
        {
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
        }
        else
        {
            dockspace_flags &= ~ImGuiDockNodeFlags.PassthruCentralNode;
        }
        
        if (dockspace_flags.HasFlag(ImGuiDockNodeFlags.PassthruCentralNode))
            window_flags |= ImGuiWindowFlags.NoBackground;

        if (!opt_padding)
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

        ImGui.Begin("DockSpace Demo", ref dockspaceOpen, window_flags);
        {
            if (!opt_padding)
                ImGui.PopStyleVar();

            if (opt_fullscreen)
                ImGui.PopStyleVar(2);

            var io = ImGui.GetIO();
            var style = ImGui.GetStyle();
            // TODO:
            var minWinSize = style.WindowMinSize.X;
            style.WindowMinSize.X = 370.0f;
            
            //if (io.ConfigFlags & ImGuiConfigFlags.DockingEnable)
            if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.DockingEnable))
            {
                var dockspace_id = ImGui.GetID("MyDockSpace");
                ImGui.DockSpace(dockspace_id, Vector2.Zero, dockspace_flags);
            }

            style.WindowMinSize.X = minWinSize;

            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    /*if (ImGui.MenuItem("New", "Ctrl+N"))
                        NewScene();

                    if (ImGui.MenuItem("Open...", "Ctrl+O"))
                        OpenScene();

                    if (ImGui.MenuItem("Save As...", "Ctrl+Shift+S"))
                        SaveSceneAs();*/
                    
                    if (ImGui.MenuItem("Exit"))
                        Application.Instance.Close();
                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar();
            }

            // ImGui Drawing Here

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.Begin("Viewport");
            {
                _viewportFocused = ImGui.IsWindowFocused();
                _viewportHovered = ImGui.IsWindowHovered();
                //Application::Get().GetImGuiLayer()->SetBlockEvents(!m_ViewportFocused || !m_ViewportHovered);

                _viewportSize = ImGui.GetContentRegionAvail();

                // TODO: Image drawing 
                //var textureID = m_Framebuffer->GetColorAttachmentRendererID();
                //ImGui::Image((void*)textureID, ImVec2{ m_ViewportSize.x, m_ViewportSize.y }, ImVec2{ 0, 1 }, ImVec2{ 1, 0 });
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