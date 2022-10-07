using System.Numerics;
using ImGuiNET;

namespace CouscousEngine.rlImGui;

public static class Docking
{
    public static bool IsViewportFocused;
    public static bool IsViewportHovered;
    
    public static void Draw(Action drawViewport, Action? drawGUI)
    {
        var dockspaceOpen = true;
        var opt_fullscreen = true;
        var opt_padding = false;
        var dockspace_flags = ImGuiDockNodeFlags.None;

        var window_flags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
        if (opt_fullscreen)
        {
            var viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            window_flags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            window_flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
        }
        else
        {
            dockspace_flags &= ~ImGuiDockNodeFlags.PassthruCentralNode;
        }

        if (dockspace_flags.HasFlag(ImGuiDockNodeFlags.PassthruCentralNode))
            window_flags |= ImGuiWindowFlags.NoBackground;

        if (!opt_padding)
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));

        ImGui.Begin("DockSpace", ref dockspaceOpen, window_flags);
        {
            if (!opt_padding)
                ImGui.PopStyleVar();

            if (opt_fullscreen)
                ImGui.PopStyleVar(2);

            var io = ImGui.GetIO();

            if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.DockingEnable))
            {
                var dockspace_id = ImGui.GetID("MyDockSpace");
                ImGui.DockSpace(dockspace_id, new Vector2(0.0f, 0.0f), dockspace_flags);
            }

            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Exit"))
                    {
                        
                    }
                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar();
            }

            drawGUI?.Invoke();

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            ImGui.Begin("Viewport");
            {
                IsViewportFocused = ImGui.IsWindowFocused();
                IsViewportHovered = ImGui.IsWindowHovered();
                drawViewport.Invoke();
            }
            ImGui.End();
            ImGui.PopStyleVar();
        }
        ImGui.End();
    }
}