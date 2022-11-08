using System.ComponentModel;
using System.Reflection;

namespace CouscousEngine.Utils;

public static class Util
{
    public static string? GetDescription<T>(this T enumerationValue)
        where T : struct
    {
        var type = enumerationValue.GetType();
        if (!type.IsEnum)
            throw new ArgumentException("EnumerationValue must be of Enum type", 
                nameof(enumerationValue));
        
        var memberInfo = type.GetMember(enumerationValue.ToString() ?? string.Empty);
        if (memberInfo.Length > 0)
        {
            var attrs = memberInfo[0].GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attrs.Length > 0)
                return ((DescriptionAttribute)attrs[0]).Description;
            
        }
        return enumerationValue.ToString();
    }
}