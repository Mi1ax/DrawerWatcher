using CouscousEngine.GUI;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CouscousEngine.Editor;

public class Scene : UUID
{
    public Dictionary<Guid, Button> Buttons { get; set; }

    public Scene()
    {
        Buttons = new Dictionary<Guid, Button>();
    }

    public Button AddObject(Button visual)
    {
        Buttons.Add(visual.Guid, visual);
        return Buttons[visual.Guid];
    }

    public Button GetObject(Guid guid) => Buttons[guid];

    public Button? GetObject(string objectName)
        => Buttons.Values.FirstOrDefault(visualsValue => visualsValue.UniqueName == objectName);

    public void Update()
    {
        foreach (var visual in Buttons.Values)
            visual.Update();
    }

    public static void Save(Scene scene, string filename)
    {
        if (!Directory.Exists("Scenes"))
            Directory.CreateDirectory("Scenes");

        var filepath = "Scenes/" + filename + ".yaml";

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        File.WriteAllText(filepath, serializer.Serialize(scene));
    }

    public static Scene Load(string filename)
    {
        if (!Directory.Exists("Scenes"))
            Directory.CreateDirectory("Scenes");
        
        var filepath = "Scenes/" + filename + ".yaml";
        
        if (!File.Exists(filepath))
            return new Scene();
        
        var text = File.ReadAllText(filepath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<Scene>(text);
    }
}