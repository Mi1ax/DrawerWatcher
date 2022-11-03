using CouscousEngine.Core;
using ImGuiNET;

namespace CouscousEngine.rlImGui;

public class ImGuiLayer : Layer
{
    public ImGuiLayer() 
        : base("ImGuiLayer")
    {
    }

    public override void OnAttach()
    {
        rlImGui.Setup();
    }

    public override void OnDetach()
    {
        rlImGui.Shutdown();
    }

    public override void OnUpdate(float deltaTime)
    {
        
    }

    public override bool OnEvent()
    {
        var io = ImGui.GetIO();
        return io.WantCaptureMouse || io.WantCaptureKeyboard;
    }
}