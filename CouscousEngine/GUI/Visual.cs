using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Editor;
using CouscousEngine.Utils;

namespace CouscousEngine.GUI;

public abstract class Visual : UUID
{
    [Inspectable] protected Size Size { get; set; }
    [Inspectable] protected Vector2 Position { get; set; }
    
    protected Visual(string uniqueObjectName) 
        : base(uniqueObjectName)
    {
        Size = new Size(125, 25);
        var windowSize = Application.Instance.GetSize();
        Position = new Vector2(
            windowSize.Width / 2f - Size.Width / 2f, 
            windowSize.Height / 2f - Size.Height / 2f);
    }
    
    protected Visual(string uniqueObjectName, Size size, Vector2 position)
        : base(uniqueObjectName)
    {
        Size = size;
        Position = position;
    }
    
    public abstract void Update();
}