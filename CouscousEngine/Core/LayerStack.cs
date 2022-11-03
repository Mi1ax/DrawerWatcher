namespace CouscousEngine.Core;

public class LayerStack
{
    public readonly List<Layer> Layers;

    public LayerStack()
    {
        Layers = new List<Layer>();
    }

    public void PushLayer(Layer layer)
        => Layers.Add(layer);

    public void PopLayer(Layer layer)
        => Layers.Remove(layer);
}