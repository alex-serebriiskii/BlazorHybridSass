using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace BlazorHybridSass;

class BlazorTree
{
    private readonly string tagPattern = @"<(?<tagname>[a-zA-Z][\w:\-]*)(?<attributename>\s+[\w:\-]+(?<attributevalue>\s*=\s*(?:"".*?""|'.*?'|[^'""\s>]+))?)*\s*(?<selfclosingtag>\/?)>|<\/(?<closingtagname>[a-zA-Z][\w:\-]*)>";
    private BlazorNode? root {get;set;}
    private BlazorNode? currentNode {get;set;}
    private Dictionary<string,string> tags = new Dictionary<string, string>();
    public void BuildTree(FileInfo file)
    {
        string content = File.ReadAllText(file.FullName);
        var matches = Regex.Matches(content, tagPattern, RegexOptions.Multiline);
        root = new BlazorNode() { Name = file.Name };
        currentNode = root;
        foreach (Match match in matches)
        {
            //Check if opening tag
            if(match.Groups["tagname"].Success)
            {
                var tagname = match.Groups["tagname"].Value;
                BlazorNode activeNode = new BlazorNode(){Name = tagname};
                activeNode.Parent = currentNode;
                currentNode.Children.Add(activeNode);
                //Record a new tag if it comes up
                if(!tags.ContainsKey(tagname))
                {
                    tags.Add(tagname, tagname);
                }
                //Handle self closing tag
                if((match.Groups["selfclosingtag"].Success && !string.IsNullOrEmpty(match.Groups["selfclosingtag"].Value) )|| (match.Groups[""].Success && match.Groups[""].Value.EndsWith("\\")))
                //Regex doesn't handle the case of an unquoted attribute right before the end of a self closing tag (ie ...lastattribute=value/>), so we do an explicit check
                {
                    //mark node closed, stay at current level
                    activeNode.IsOpen = false;
                    continue;
                }
                //Tag isn't self closing, handle moving down tree
                currentNode = activeNode;
            }
            else if(match.Groups["closingtagname"].Success)
            {
                var closingtagname = match.Groups["closingtagname"].Value;
                if(closingtagname == currentNode.Name)
                {
                    currentNode.IsOpen = false;
                    currentNode = currentNode.Parent;
                }else{
                    throw new Exception("Closing tag \"" + closingtagname + "\" doesn't match current open tag \"" + currentNode.Name + "\"");
                }
            }
        }
    }
}