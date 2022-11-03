using CouscousEngine.Core;
using ImGuiNET;

namespace Sandbox;

public class SandboxLayer : Layer
{
    public SandboxLayer() 
        : base("SandboxLayer")
    {
        
    }

    public override void OnUpdate(float deltaTime)
    {
        
    }

    public override void OnImGuiUpdate()
    {
        ImGui.ShowDemoWindow();
        ImGui.ShowFontSelector("Font");
    }

    public override bool OnEvent()
    {
        return false;
    }
}