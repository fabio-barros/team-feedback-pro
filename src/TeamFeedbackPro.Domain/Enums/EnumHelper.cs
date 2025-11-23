using System.ComponentModel;

namespace TeamFeedbackPro.Domain.Enums;

public static class EnumHelper
{
    public static T? GetAttribute<T>(this Enum value) where T : Attribute {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
        return attributes.Length > 0 ?
            (T) attributes[0] :
            null;
    }

    public static string ToDescription(this Enum value) {
        var attribute = value.GetAttribute<DescriptionAttribute>();
        return attribute == null ? value.ToString() : attribute.Description;
    }
}