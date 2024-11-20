using MultiLanguage;

// 前端内容
var path = @"D:\work\BingoV2\src\Frontend\bingov2\src";
var patterns = new string[] { ".vue", ".ts" };
var excludeFiles = new string[] { "enumToString.ts" };
// var lines = FileContentHelper.GetCNContent(path, patterns, QuoteType.Both, excludeFiles);

// File.WriteAllLines("./front_cn.txt", lines);

// // 后端内容

// path = @"D:\work\BingoV2\src\Bingo.Service.Open";
// patterns = new string[] { ".cs" };
// lines = FileContentHelper.GetCNContent(path, patterns, QuoteType.Both);

// File.WriteAllLines("./back_cn.txt", lines);


var jsonPath = @"D:\work\BingoV2\src\Frontend\bingov2\public\locales\zh-CN.json";
var sourceData = FileContentHelper.ReadJsonToDictionary(jsonPath);

var matchTemplates = new string[]
{
    "label=\"{0}\"",
    "placeholder=\"{0}\"",
    "v-tooltip=\"'{0}'\"",
    "Name: '{0}'",
    "title: '{0}'",
    "subTitle: '{0}'",
};

var replaceTemplates = new string[]
{
    ":label=\"$t('{0}')\"",
    ":placeholder=\"$t('{0}')\"",
    "v-tooltip=\"$t('{0}')\"",
    "Name: this.trans('{0}')",
    "title: this.trnas('{0}')",
    "subTitle: this.trans('{0}')",
};

FileContentHelper.ReplaceContent(path, patterns, sourceData, matchTemplates, replaceTemplates, excludeFiles);