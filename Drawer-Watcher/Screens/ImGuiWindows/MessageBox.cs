using System.Numerics;
using Drawer_Watcher.Localization;
using ImGuiNET;

namespace Drawer_Watcher.Screens.ImGuiWindows;

public enum MessageBoxButtons
{
    YesNo,
    Ok
}

public enum MessageBoxResult
{
    None, Ok, Yes, No
}

public static class MessageBox
{
    private static bool _open;
    private static string _title = "MessageBox";
    private static string _message = "Message";
    private static MessageBoxButtons _buttons;
    private static Action<MessageBoxResult>? _onResult;

    public static bool IsOpen() => _open;
    
    public static void Show(
        string title, string message, 
        MessageBoxButtons buttons, Action<MessageBoxResult>? onResult = null)
    {
        _title = title;
        _message = message;
        _buttons = buttons;
        _open = true;
        _onResult = onResult;
    }

    public static void OnImGuiUpdate()
    {
        if (_open)
            ImGui.OpenPopup($"{_title}##messageBox");
        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
        if (ImGui.BeginPopupModal($"{_title}##messageBox", ref _open, 
                ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.Text(_message);
            switch (_buttons)
            {
                case MessageBoxButtons.YesNo:
                    Button(MessageBoxResult.Yes);
                    ImGui.SameLine();
                    Button(MessageBoxResult.No);
                    break;
                case MessageBoxButtons.Ok:
                    Button(MessageBoxResult.Ok);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_buttons), _buttons, null);
            }
            ImGui.EndPopup();
        }
    }
    
    private static void Button(MessageBoxResult button)
    {
        if (!ImGui.Button(LanguageSystem.GetLocalized(button.ToString()))) 
            return;
        _open = false;
        ImGui.CloseCurrentPopup();
        _onResult?.Invoke(button);
    }
}