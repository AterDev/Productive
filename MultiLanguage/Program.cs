using MultiLanguage;

var path = @"";
var patterns = new string[] { ".vue", ".ts" };
var lines = FileContentHelper.GetCNContent(path, patterns, QuoteType.Both);

//输出
foreach (var item in lines)
{
    Console.WriteLine(item);
}

var jsonPath = @"";
var sourceData = FileContentHelper.ReadJsonToDictionary(jsonPath);

var matchTemplates = new string[]
{
    "label:'{0}'",
    "placeholder:'{0}'"
};

var replaceTemplates = new string[]
{
    ":label:'{0}'",
    ":placeholder:'{0}'"
};

FileContentHelper.ReplaceContent(path, patterns, sourceData, matchTemplates, replaceTemplates);