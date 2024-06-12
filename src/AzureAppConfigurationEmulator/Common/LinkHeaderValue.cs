using System.Text;
using System.Text.RegularExpressions;

namespace AzureAppConfigurationEmulator.Common;

public partial class LinkHeaderValue
{
    private readonly IDictionary<string, ICollection<Uri>> _values;

    private LinkHeaderValue(IDictionary<string, ICollection<Uri>> values)
    {
        _values = values;
    }

    public IEnumerable<Uri>? Next => _values.TryGetValue("next", out var uris) ? uris : null;

    public IEnumerable<Uri>? Prev => _values.TryGetValue("prev", out var uris) ? uris : null;

    public static LinkHeaderValue Parse(string input)
    {
        var values = new Dictionary<string, ICollection<Uri>>();

        if (!string.IsNullOrEmpty(input))
        {
            if (input.Split(',') is { Length: > 0 } links)
            {
                foreach (var link in links)
                {
                    if (RelRegex().Match(link) is { Success: true } rel &&
                        UriRegex().Match(link) is { Success: true } uri)
                    {
                        if (values.TryGetValue(rel.Value, out var uris))
                        {
                            uris.Add(new Uri(uri.Value, UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            values.Add(rel.Value, [new Uri(uri.Value, UriKind.RelativeOrAbsolute)]);
                        }
                    }
                }
            }
        }

        return new LinkHeaderValue(values);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var (rel, uris) in _values)
        {
            builder.AppendJoin(", ", uris.Select(uri => $"<{uri}>; rel=\"{rel}\""));
        }

        return builder.ToString();
    }

    [GeneratedRegex("(?<=rel=\").+?(?=\")")]
    private static partial Regex RelRegex();

    [GeneratedRegex("(?<=<).+?(?=>)")]
    private static partial Regex UriRegex();
}
