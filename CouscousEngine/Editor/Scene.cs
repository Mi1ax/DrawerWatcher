using CouscousEngine.GUI;

namespace CouscousEngine.Editor;

public class Scene : UUID
{
    public Dictionary<Guid, Visual> Visuals { get; set; }

    public Scene(string sceneName) 
        : base(sceneName)
    {
        Visuals = new Dictionary<Guid, Visual>();
    }

    public Visual AddObject(Visual visual)
    {
        Visuals.Add(visual.GetGuid(), visual);
        return Visuals[visual.GetGuid()];
    }

    public Visual GetObject(Guid guid) => Visuals[guid];

    public Visual? GetObject(string objectName)
        => Visuals.Values.FirstOrDefault(visualsValue => visualsValue.GetName() == objectName);

    public void Update()
    {
        foreach (var visual in Visuals.Values)
            visual.Update();
    }
}