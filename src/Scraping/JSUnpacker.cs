using System.Text.RegularExpressions;

namespace Ocluse.LiquidSnow.Scraping;
/// <summary>
/// Unpacks packed JavaScript code.
/// </summary>
public partial class JSUnpacker
{
    /// <summary>
    /// Detects if the source contains packed JavaScript code.
    /// </summary>
    public static bool Detect(string source)
    {
        return PackedRegex().IsMatch(source);
    }

    /// <summary>
    /// Detects if the supplied sources contain packed JavaScript code.
    /// </summary>
    public static List<string> Detect(params string[] scriptBlocks)
    {
        return [.. scriptBlocks.Where(Detect)];
    }

    /// <summary>
    /// Detects if the supplied sources contain packed JavaScript code.
    /// </summary>
    public static List<string> Detect(IEnumerable<string> scriptBlocks)
    {
        return [.. scriptBlocks.Where(Detect)];
    }

    /// <summary>
    /// Attempts to unpack the supplied packed JavaScript code.
    /// This method returns all available unpacked code.
    /// </summary>
    public static IEnumerable<string> UnpackAll(string scriptBlock)
    {
        if (!Detect(scriptBlock))
        {
            return [];
        }
        else
        {
            return PerformUnpack(scriptBlock);
        }
    }

    /// <summary>
    /// Unpacks available packed JavaScript code in the supplied <paramref name="scriptBlocks"/>
    /// </summary>
    public static List<string> Unpack(IEnumerable<string> scriptBlocks)
    {
        return [.. scriptBlocks.SelectMany(UnpackAll)];
    }

    /// <summary>
    /// Attempts to unpack the supplied JavaScript code and returns a single string or null if no packed code was detected.
    /// </summary>
    public static string? Unpack(string scriptBlock)
    {
        var unpacked = UnpackAll(scriptBlock).ToList();

        if (unpacked.Count == 0)
        {
            return null;
        }

        return string.Join('\n', unpacked);
    }

    private static IEnumerable<string> PerformUnpack(string scriptBlock)
    {
        var matches = PackedExtractRegex().Matches(scriptBlock);

        foreach (Match match in matches)
        {
            var payload = match.Groups[1].Value;
            var symTab = match.Groups[4].Value.Split('|');
            var radix = int.Parse(match.Groups[2].Value);
            var count = int.Parse(match.Groups[3].Value);

            var unbaser = new Unbaser(radix);

            var unpacked = UnpackReplaceRegex().Replace(payload, m =>
            {
                var word = m.Value;
                var unbase = unbaser.Unbase(word);

                if (unbase < count)
                {
                    return symTab[unbase];
                }
                else
                {
                    return word;
                }
            });

            yield return unpacked;

        }
    }

    [GeneratedRegex("eval[(]function[(]p,a,c,k,e,[r|d]?", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-GB")]
    private static partial Regex PackedRegex();

    [GeneratedRegex("[}][(]'(.*)', *(\\d+), *(\\d+), *'(.*?)'[.]split[(]'[|]'[)]", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-GB")]
    private static partial Regex PackedExtractRegex();

    [GeneratedRegex("\\b\\w+\\b", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-GB")]
    private static partial Regex UnpackReplaceRegex();
}