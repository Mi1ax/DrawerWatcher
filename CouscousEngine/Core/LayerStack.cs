namespace CouscousEngine.Core;

public class LayerStack
{
    private uint _layerInsertCount;
    public readonly List<Layer> Layers;

    public LayerStack()
    {
        Layers = new List<Layer>();
    }

    public void PushLayer(Layer layer)
    {
        Layers.Add(layer);
        _layerInsertCount++;
    }

    public void PopLayer(Layer layer)
        => Layers.Remove(layer);
}