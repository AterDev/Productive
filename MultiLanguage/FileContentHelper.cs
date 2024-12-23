﻿using System.Diagnostics.CodeAnalysis;
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
    public static List<string> GetCNContent(string path, string[] patterns, QuoteType quate = QuoteType.Double, string[]? excludeFiles = null)
    {
        var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(file => patterns.Contains(Path.GetExtension(file)))
            .ToList();

        if (excludeFiles != null)
        {
            files = files.Where(file => !excludeFiles.Contains(Path.GetFileName(file))).ToList();
        }
        var pattern = quate switch
        {
            QuoteType.Single => @"'[^'\u4e00-\u9fa5]*[\u4e00-\u9fa5]+[^'\u4e00-\u9fa5]*'",
            QuoteType.Double => @"""[^""\u4e00-\u9fa5]*[\u4e00-\u9fa5]+[^""\u4e00-\u9fa5]*""",
            QuoteType.Both => @"(['""])([^\r\n'""]*[\u4e00-\u9fa5][^\r\n'""]*)\1",
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
                var value = match.Value.Trim('\'', '"');
                contentSet.Add(value);
            }
        }
        return contentSet.ToList();
    }

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
    public static void ReplaceContent(string path, string[] patterns, Dictionary<string, string> data, string[] matchTemplates, string[] replaceTemplates, string[]? excludeFiles = null)
    {
        if (matchTemplates.Length != replaceTemplates.Length)
        {
            throw new ArgumentException("matchTemplates.Length != replaceTemplates.Length");
        }

        var files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(file => patterns.Contains(Path.GetExtension(file)))
            .ToList();
        if (excludeFiles != null)
        {
            files = files.Where(file => !excludeFiles.Contains(Path.GetFileName(file))).ToList();
        }
        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            foreach (var item in data)
            {
                for (var i = 0; i < matchTemplates.Length; i++)
                {
                    var match = string.Format(matchTemplates[i], item.Value);
                    var replace = string.Format(replaceTemplates[i], item.Key);
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