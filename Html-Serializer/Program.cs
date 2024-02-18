using Html_Serializer;
using System.Text.Json;
using System.Text.RegularExpressions;


static HtmlElement ParseHtmlStrings(List<string> htmlStrings)
{
    var root = new HtmlElement();
    var currentElement = root;

    foreach (var line in htmlStrings)
    {
        var firstWord = line.Split(' ')[0];

        if (firstWord == "/html")
        {
            break; // Reached end of HTML
        }
        else if (firstWord.StartsWith("/"))
        {
            if (currentElement.Parent != null) // Make sure there is a valid parent
            {
                currentElement = currentElement.Parent; // Go to previous level in the tree
            }
        }
        else if (HtmlHelper.Instance.HtmlTags.Contains(firstWord))
        {
            var newElement = new HtmlElement();
            newElement.Name = firstWord;

            // Handle attributes
            var restOfString = line.Remove(0, firstWord.Length);
            var attributes = Regex.Matches(restOfString, "([a-zA-Z]+)=\\\"([^\\\"]*)\\\"")
                .Cast<Match>()
                .Select(m => $"{m.Groups[1].Value}=\"{m.Groups[2].Value}\"")
                .ToList();

            if (attributes.Any(attr => attr.StartsWith("class")))
            {
                // Handle class attribute
                var attributesClass = attributes.First(attr => attr.StartsWith("class"));
                var classes = attributesClass.Split('=')[1].Trim('"').Split(' ');
                newElement.Classes.AddRange(classes);
            }

            newElement.Attributes.AddRange(attributes);

            // Handle ID
            var idAttribute = attributes.FirstOrDefault(a => a.StartsWith("id"));
            if (!string.IsNullOrEmpty(idAttribute))
            {
                newElement.Id = idAttribute.Split('=')[1].Trim('"');
            }

            newElement.Parent = currentElement;
            currentElement.Children.Add(newElement);

            // Check if self-closing tag
            if (line.EndsWith("/") || HtmlHelper.Instance.HtmlVoidTags.Contains(firstWord))
            {
                currentElement = newElement.Parent;
            }
            else
            {
                currentElement = newElement;
            }
        }
        else
        {
            // Text content
            currentElement.InnerHtml = line;
        }
    }

    return root;
}

static void PrintHtmlTree(HtmlElement element, int depth)
{
    Console.WriteLine(new string(' ', depth * 4) + element.Name+" "+"id:"+element.Id);

    foreach (var child in element.Children)
    {
        PrintHtmlTree(child, depth + 1);
    }
}
static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
var html = await Load("https://www.discountbank.co.il/");
var clean = new Regex("\\s+").Replace(html, " ");
var lines = new Regex("<(.*?>)").Split(clean).Where(l => l.Length > 0);
var root = ParseHtmlStrings(lines.ToList()); 
PrintHtmlTree(root, 0);
string selectorString = "div#lang-switcher";
Selector parsedSelector = Selector.CreateTree(selectorString);

Console.WriteLine("Parsed Selector Attributes:");
Console.WriteLine("TagName: " + parsedSelector.TagName);
Console.WriteLine("Id: " + parsedSelector.Id);
Console.WriteLine("Classes: " + string.Join(", ", parsedSelector.Classes));
Console.WriteLine("Parent: " + parsedSelector.Parent);
Console.WriteLine("Child: " + parsedSelector.Child);

List<HtmlElement> r = new List<HtmlElement>();
List<HtmlElement> result = HtmlElement.Search(root, parsedSelector, r);

Console.ReadLine();