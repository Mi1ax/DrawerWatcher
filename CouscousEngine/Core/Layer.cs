namespace CouscousEngine.Core;

public abstract class Layer
{
    public Guid ID { get; }
    public string DebugName { get; }

    protected Layer(string name)
    {
        DebugName = name;
        ID = Guid.NewGuid();
    }

    public virtual void OnAttach() {}
    public virtual void OnDetach() {}
    public abstract void OnUpdate(float deltaTime);
    public abstract bool OnEvent();
    public virtual void OnImGuiUpdate() {}
}