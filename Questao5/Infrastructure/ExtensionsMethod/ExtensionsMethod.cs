using System.Reflection;
using System.Runtime.Serialization;

namespace Questao5.Infrastructure
{
    public static class ExtensionsMethod
    {
        public static string GetEnumMemberValue<T>(this T enumValue) where T : Enum
        {
            return enumValue.GetType()
                            .GetField(enumValue.ToString())
                            ?.GetCustomAttribute<EnumMemberAttribute>()
                            ?.Value ?? enumValue.ToString();
        }

        public static T ToEnum<T>(this string value) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
                if (attribute?.Value == value)
                {
                    return (T)Enum.Parse(typeof(T), field.Name);
                }
            }

            throw new ArgumentException($"Requested value '{value}' was not found in enum {typeof(T).Name}.");
        }
    }
}
