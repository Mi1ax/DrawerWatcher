using System.Numerics;
using Drawer_Watcher.Managers;
using ImGuiNET;

namespace Drawer_Watcher.Panels;

public class ChatPanel
{
    public bool DisableInput = false;
    
    private static readonly List<string> _chat = new();

    private string _text = string.Empty;

    public static void AddMessage(string nickname, string text)
    {
        _chat.Add($"{nickname}: {text}");
    }

    public static void AddToLastMessage(string value)
    {
        if (!_chat[^1].Contains(value))
            _chat[^1] += value;
    }

    public static void ClearChat()
        => _chat.Clear();

    public void OnImGuiUpdate(ImGuiWindowFlags flags = ImGuiWindowFlags.None)
    {
        ImGui.Begin("Chat", flags);
        ImGui.Text("Chat");
        ImGui.Separator();
        var footerHeightToReserve = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
        ImGui.BeginChild("Scrollbar", new Vector2(0, -footerHeightToReserve), false, ImGuiWindowFlags.HorizontalScrollbar);
        foreach (var text in _chat)
            ImGui.Text(text);
        
        if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
            ImGui.SetScrollHereY(1.0f);
        ImGui.EndChild();
        
        ImGui.Separator();
        if (!DisableInput)
        {
            if (ImGui.InputText("Input", ref _text, 128, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                if (Player.ApplicationOwner == null) return;
                MessageHandlers.SendMessageInChat(Player.ApplicationOwner.ID, _text);
                _text = "";
                ImGui.SetKeyboardFocusHere(-1);
            }
        }
        ImGui.End();
    }
}