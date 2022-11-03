using System.Numerics;
using CouscousEngine.Core;
using CouscousEngine.Utils;

namespace CouscousEngine.GUI;

public abstract class Visual
{
    public abstract void OnUpdate(float deltaTime);
    public virtual bool OnEvent() => false;
}