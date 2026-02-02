using System.Globalization;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HireHub.Core.Utils.Common;

public static class Helper
{
    public static T Map<F, T>(F fromData) where F : class where T : class, new()
    {
        return Map(fromData, new T());
    }

    public static T Map<F, T>(F fromData, T toData) where F : class where T : class, new()
    {
        Type fromType = typeof(F);
        Type toType = typeof(T);

        return Map(fromData, fromType, toData, toType);
    }

    private static T Map<F, T>(F fromData, Type fromType, T toData, Type toType) where F : class where T : class, new()
    {
        foreach (var fromProperty in fromType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var toProperty = toType.GetProperty(fromProperty.Name);
            if (toProperty == null) continue;
            if (!toProperty.CanWrite) continue;

            object? value = fromProperty.GetValue(fromData);

            if (    
                    fromProperty.PropertyType.IsClass && 
                    (    
                        fromProperty.PropertyType.IsArray || 
                        (fromProperty.PropertyType.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(fromProperty.PropertyType))
                    )
               )
            {
                value = MapCollectionOrArray(value, fromProperty.PropertyType, toProperty.PropertyType);
            }
            else if (fromProperty.PropertyType.IsClass && fromProperty.PropertyType != typeof(string))
            {
                var fromPropertyData = fromProperty.GetValue(fromData);
                var toPropertyData = toProperty.GetValue(toData) ?? Activator.CreateInstance(toProperty.PropertyType);

                value = (toPropertyData != null && fromPropertyData != null) ? 
                    Map(fromPropertyData, fromProperty.PropertyType, toPropertyData, toProperty.PropertyType) : 
                    null;
            }

            toProperty.SetValue(toData, value);
        }

        return toData;
    }

    private static object? MapCollectionOrArray(object? fromValue, Type fromType, Type toType)
    {
        if (fromValue == null) return null;

        // Handle arrays
        if (fromType.IsArray && toType.IsArray)
        {
            var fromArray = (Array)fromValue;
            var elementType = toType.GetElementType();
            var toArray = Array.CreateInstance(elementType!, fromArray.Length);

            for (int i = 0; i < fromArray.Length; i++)
            {
                var fromElement = fromArray.GetValue(i);
                var toElement = elementType != typeof(string) ?
                    Activator.CreateInstance(elementType!) : fromElement;

                var mappedElement = (fromElement != null && toElement != null)
                    ? Map(fromElement, fromElement.GetType(), toElement, elementType!)
                    : null;

                toArray.SetValue(mappedElement, i);
            }

            return toArray;
        }

        // Handle generic collections (e.g., List<T>)
        if (fromType.IsGenericType &&
            typeof(System.Collections.IEnumerable).IsAssignableFrom(fromType))
        {
            var fromCollection = (System.Collections.IEnumerable)fromValue;
            var toCollection = (System.Collections.IList?)Activator.CreateInstance(toType);
            var elementType = toType.GetGenericArguments()[0];

            foreach (var fromElement in fromCollection)
            {
                var toElement = elementType != typeof(string) ?
                    Activator.CreateInstance(elementType!) : fromElement;

                var mappedElement = (fromElement != null && toElement != null)
                    ? Map(fromElement, fromElement.GetType(), toElement, elementType)
                    : null;

                toCollection!.Add(mappedElement);
            }

            return toCollection;
        }

        return null;
    }


    public static readonly ValueConverter<TimeOnly, string> TimeConverter = new
    (
        v => v.ToString("HH:mm", CultureInfo.InvariantCulture),           // TimeOnly → string
        v => TimeOnly.ParseExact(v, new string[] { "HH:mm", "hh:mm tt" }) // string → TimeOnly
    );

    public static readonly ValueConverter<List<string>, string?> ListConverter = new
    (
        v => v == null || v.Count == 0
            ? null
            : string.Join(",", v),                           // List → comma-separated string
        v => string.IsNullOrEmpty(v)
            ? new List<string>()
            : v.Split(',', StringSplitOptions.None).ToList() // string → List<string>
    );

    public static ValueConverter<T, string> EnumConverter<T>() where T : Enum => new
    (
        v => v.ToString(),                // Enum → string
        v => (T)Enum.Parse(typeof(T), v, true)  // string → Enum
    );

}
