using System.Text.Json.Serialization.Metadata;

namespace AzureAppConfigurationEmulator.Common;

/// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/custom-contracts#example-ignore-properties-with-a-specific-type">Customize a JSON contract</seealso>
public class SelectJsonTypeInfoModifier(IEnumerable<string>? names = null)
{
    public void Modify(JsonTypeInfo type)
    {
        if (type.Kind == JsonTypeInfoKind.Object)
        {
            for (var i = type.Properties.Count - 1; i >= 0; i--)
            {
                if (names?.Contains(type.Properties[i].Name) is false)
                {
                    type.Properties.RemoveAt(i);
                }
            }
        }
    }
}
