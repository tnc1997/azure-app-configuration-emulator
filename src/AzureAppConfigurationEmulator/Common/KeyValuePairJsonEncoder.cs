using System.Text.Json;
using System.Text.Json.Nodes;

namespace AzureAppConfigurationEmulator.Common;

public class KeyValuePairJsonEncoder : IKeyValuePairJsonEncoder
{
    public JsonDocument Encode(
        IEnumerable<KeyValuePair<string, string?>> pairs,
        string? prefix = null,
        string? separator = null)
    {
        JsonNode root = new JsonObject();

        foreach (var (key, value) in pairs)
        {
            var keys = key.Split(separator).ToList();

            if (!string.IsNullOrEmpty(prefix))
            {
                if (keys[0] == prefix)
                {
                    keys.RemoveAt(0);
                }
                else if (keys[0].StartsWith(prefix))
                {
                    keys[0] = keys[0][prefix.Length..];
                }
            }

            var current = root;

            for (var i = 0; i < keys.Count; i++)
            {
                if (int.TryParse(keys[i], out var index))
                {
                    if (i == keys.Count - 1)
                    {
                        current.AsArray().Insert(index, value);

                        break;
                    }

                    if (current.AsArray().ElementAtOrDefault(index) is not { } next)
                    {
                        if (int.TryParse(keys[i + 1], out _))
                        {
                            next = new JsonArray();
                        }
                        else
                        {
                            next = new JsonObject();
                        }

                        current.AsArray().Insert(index, next);
                    }

                    current = next;
                }
                else
                {
                    if (i == keys.Count - 1)
                    {
                        current[keys[i]] = value;
                        
                        break;
                    }

                    if (current[keys[i]] is not { } next)
                    {
                        if (int.TryParse(keys[i + 1], out _))
                        {
                            next = new JsonArray();
                        }
                        else
                        {
                            next = new JsonObject();
                        }

                        current[keys[i]] = next;
                    }

                    current = next;
                }
            }
        }

        return root.Deserialize<JsonDocument>()!;
    }
}
