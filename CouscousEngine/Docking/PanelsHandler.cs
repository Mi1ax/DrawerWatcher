using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Shapes;
using CouscousEngine.Utils;

namespace CouscousEngine.Docking;

public class PanelsHandler
{
    private Size _windowSize;
    private readonly List<Panel> _panels;

    public PanelsHandler()
    {
        _windowSize = Application.Instance.GetSize();
        _panels = new List<Panel>();
    }

    public Panel AddPanel(float percent)
    {
        var startPosX = _panels.Sum(dock_ => dock_.Position.X + dock_.Size.Width);

        var rect = new Rectangle
        {
            Size = new Size(_windowSize.Width * percent, _windowSize.Height),
            Position = new Vector2(startPosX, 0),
            Color = Color.GRAY
        };
        var dock = new Panel(rect);
        _panels.Add(dock);
        return _panels.Last();
    }

    public Panel GetDock(int index) => _panels[index];

    public void Draw()
    {
        foreach (var dock in _panels)
            dock.Draw();
    }
}