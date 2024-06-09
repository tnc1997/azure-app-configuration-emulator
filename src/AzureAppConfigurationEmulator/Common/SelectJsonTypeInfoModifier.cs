using System.Text.Json.Serialization.Metadata;

namespace AzureAppConfigurationEmulator.Common;

/// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/custom-contracts#example-ignore-properties-with-a-specific-type">Customize a JSON contract</seealso>
public class SelectJsonTypeInfoModifier
{
    public SelectJsonTypeInfoModifier(IEnumerable<string>? names = null)
    {
        if (names is not null)
        {
            foreach (var name in names)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    Names.Add(name);
                }
            }
        }
    }

    private ICollection<string> Names { get; } = [];

    public void Modify(JsonTypeInfo type)
    {
        if (type.Kind == JsonTypeInfoKind.Object)
        {
            if (Names.Count > 0)
            {
                for (var i = type.Properties.Count - 1; i >= 0; i--)
                {
                    if (!Names.Append("items").Contains(type.Properties[i].Name))
                    {
                        type.Properties.RemoveAt(i);
                    }
                }
            }
        }
    }
}
