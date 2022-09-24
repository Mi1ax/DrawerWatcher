using System.Numerics;
using System.Reflection;
using CouscousEngine.Utils;
using ImGuiNET;

namespace CouscousEngine.CCGui;

public enum InspectorHint : byte
{
    Default,
    ReadOnly
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class InspectableAttribute : Attribute
{
    public InspectorHint Hint { get; }
    public string? Name { get; }
    
    public InspectableAttribute(InspectorHint hint = InspectorHint.Default, string? name = null)
    {
        Name = name;
        Hint = hint;
    }
}

public static class Inspector
{
    public static void DrawWindow(object obj)
    {
        ImGui.Begin(obj.GetType().Name);
        {
            foreach (var propertyInfo in GetProperties(obj))
            {
                InputField(obj, propertyInfo);
            }
        }
        ImGui.End();
    }

    private static void InputField(object obj, PropertyInfo fieldInfo)
    {
        if (fieldInfo.GetCustomAttribute(typeof(InspectableAttribute)) is not InspectableAttribute attribute) 
            throw new Exception($"{nameof(InspectableAttribute)} not found in {obj}");
        
        var name = attribute.Name ?? fieldInfo.Name;
        var value = fieldInfo.GetValue(obj);
        var isReadOnly = attribute.Hint == InspectorHint.ReadOnly;

        switch (value)
        {
            case string vString:
                if (isReadOnly)
                    ImGui.Text($"{name}: {vString}");
                else
                {
                    ImGui.InputText(name, ref vString, 32);
                    fieldInfo.SetValue(obj, vString);
                }

                break;
            case Vector2 vVector2:
                ImGui.DragFloat2(name, ref vVector2);
                fieldInfo.SetValue(obj, vVector2);
                break;
            case Size vSize:
                var asVector = (Vector2) vSize;
                ImGui.DragFloat2(name, ref asVector);
                fieldInfo.SetValue(obj, new Size(asVector.X, asVector.Y));
                break;
            default:
                throw new Exception("Unknown value type!");
        }
    }
    
    private static IEnumerable<PropertyInfo> GetProperties(object obj)
    {
        return obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(prop => prop.IsDefined(typeof(InspectableAttribute)));
    }
}