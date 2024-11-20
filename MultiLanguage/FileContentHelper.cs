using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MultiLanguage;
/// <summary>
/// 内容帮助
/// </summary>
internal class FileContentHelper
{
    /// <summary>
    /// 获取中文内容
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileExt"></param>
    /// <param name="quate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static List<string> GetCNContent(string path, string[] patterns, QuoteType quate = QuoteType.Double)
    {
        var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(file => patterns.Contains(Path.GetExtension(file)))
            .ToList();

        var pattern = quate switch
        {
            QuoteType.Single => @"'[\u4e00-\u9fa5]+?'",
            QuoteType.Double => @"""[\u4e00-\u9fa5]+?""",
            QuoteType.Both => @"['""][\u4e00-\u9fa5]+?['""]",
            _ => throw new ArgumentOutOfRangeException(nameof(quate), quate, null)
        };
        var regex = new Regex(pattern, RegexOptions.Compiled);
        // hashset  存储 匹配内容 
        var contentSet = new HashSet<string>();
        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            var matches = regex.Matches(content);
            foreach (Match match in matches)
            {
                contentSet.Add(match.Value);
            }
        }
        return contentSet.ToList();
    }


    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
    [RequiresDynamicCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
    public static Dictionary<string, string> ReadJsonToDictionary(string path)
    {
        if (File.Exists(path))
        {
            var content = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(content) ?? [];
        }
        return [];
    }

    /// <summary>
    /// 模板替换
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileExt"></param>
    /// <param name="data"></param>
    /// <param name="matchTemplates"></param>
    /// <param name="replaceTemplate"></param>
    public static void ReplaceContent(string path, string[] patterns, Dictionary<string, string> data, string[] matchTemplates, string[] replaceTemplates)
    {
        if (matchTemplates.Length != replaceTemplates.Length)
        {
            throw new ArgumentException("matchTemplates.Length != replaceTemplates.Length");
        }

        var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(file => patterns.Contains(Path.GetExtension(file)))
            .ToList();
        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            foreach (var item in data)
            {
                for (var i = 0; i < matchTemplates.Length; i++)
                {
                    var match = string.Format(matchTemplates[i], item.Key);
                    var replace = string.Format(replaceTemplates[i], item.Value);
                    content = content.Replace(match, replace);
                }
            }
            File.WriteAllText(file, content);
        }
    }
}

enum QuoteType
{
    Single,
    Double,
    Both,
}