using YamlDotNet.Serialization;

namespace CouscousEngine.Editor;

public abstract class UUID
{
    [YamlIgnore] public readonly Guid Guid = Guid.NewGuid();

    private readonly string? _uniqueName;

    public string UniqueName
    {
        get => _uniqueName ?? Guid.ToString();
        init => _uniqueName = value;
    }
}