namespace CouscousEngine.Editor;

public abstract class UUID
{
    public readonly Guid _guid = Guid.NewGuid();
    public readonly string _uniqueObjectName;

    protected UUID(string uniqueObjectName) => _uniqueObjectName = uniqueObjectName;

    public Guid GetGuid() => _guid;
    public string GetName() => _uniqueObjectName;
}