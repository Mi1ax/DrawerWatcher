using System.Drawing;
using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.rlImGui;
using Drawer_Watcher.Localization;
using Drawer_Watcher.Managers;
using Drawer_Watcher.Screens;
using Drawer_Watcher.Screens.ImGuiWindows;
using ImGuiNET;
using Raylib_CsLo;
using Riptide;
using Color = CouscousEngine.Utils.Color;
using MouseButton = CouscousEngine.Core.MouseButton;
using Rectangle = CouscousEngine.Shapes.Rectangle;

namespace Drawer_Watcher.Panels;

public class ToolPanel
{
    private readonly float[] _brushesSize;
    private readonly Color[] _colors;
    private int _selectedIndex;
    private int _currentSize;

    public ToolPanel()
    {
        _colors = new []
        {
            (Color)ColorTranslator.FromHtml("#414042"),
            ColorTranslator.FromHtml("#ff675f"),
            ColorTranslator.FromHtml("#a97c50"),
            ColorTranslator.FromHtml("#ff8939"),
            ColorTranslator.FromHtml("#f9ed32"),
            ColorTranslator.FromHtml("#64bf28"),
            ColorTranslator.FromHtml("#30d8d8"),
            ColorTranslator.FromHtml("#27aae1"),
            ColorTranslator.FromHtml("#6d51f4"),
            ColorTranslator.FromHtml("#ee8aff")
        };

        _brushesSize = new []
        {
            8f, 12f, 24f
        };
    }

    public void OnImGuiUpdate(ImGuiWindowFlags flags = ImGuiWindowFlags.None)
    {
        ImGui.Begin("Tool Box", flags);
        if (Player.ApplicationOwner!.IsDrawer)
        {
            ImGui.Text(LanguageSystem.GetLocalized("CurrentColor"));
            ImGui.SameLine();
            ImGui.ColorButton("current_color", _colors[_selectedIndex], ImGuiColorEditFlags.NoTooltip);
            ImGui.NewLine();
            for (var i = 0; i < _colors.Length; i++)
            {
                ImGui.SameLine();
                if (ImGui.ColorButton($"color_button_{i}", _colors[i], ImGuiColorEditFlags.NoTooltip))
                {
                    if (Player.ApplicationOwner != null)
                        Player.ApplicationOwner.CurrentBrush.Color = _colors[i];
                    _selectedIndex = i;
                }
            }
            ImGui.SameLine();
            if (ImGui.Button(LanguageSystem.GetLocalized("ClearAll")))
                MessageHandlers.ClearPainting();
            if (SettingsData.IsHintsOn)
            {
                ImGui.SameLine();
                rlImGui.HelpMarker(LanguageSystem.GetLocalized("ClearAllHint"));
            }
            ImGui.Text($"{LanguageSystem.GetLocalized("CurrentSize")} {_brushesSize[_currentSize]}");
            ImGui.Text($"{LanguageSystem.GetLocalized("BrushSizes")}: ");
            ImGui.SameLine();
            for (var i = 0; i < _brushesSize.Length; i++)
            {
                ImGui.SameLine();
                if (ImGui.SmallButton($"{_brushesSize[i]}"))
                {
                    _currentSize = i;
                    if (Player.ApplicationOwner != null)
                        Player.ApplicationOwner.CurrentBrush.Thickness = _brushesSize[i];
                }
            }
        }
        ImGui.End();
    }
}