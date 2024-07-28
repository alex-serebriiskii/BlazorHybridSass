using System.Text.RegularExpressions;

namespace BlazorHybridSass;

class BlazorTree
{
    private readonly string tagPattern = @"<(?<tagname>[a-zA-Z][\w:\-]*)(?<attributename>\s+[\w:\-]+(?<attributevalue>\s*=\s*(?:"".*?""|'.*?'|[^'""\s>]+))?)*\s*(?<selfclosingtag>\/?)>|<\/(?<closingtagname>[a-zA-Z][\w:\-]*)>";
    private BlazorNode? root {get;set;}
    private BlazorNode? currentNode {get;set;}
    private Dictionary<string,string> tags = new Dictionary<string, string>();
    private void BuildTree(FileInfo file)
    {
        string content = File.ReadAllText(file.FullName);
        var matches = Regex.Matches(content, tagPattern);
        foreach (Match match in matches)
        {
            //Check if opening tag
            if(match.Groups["tagname"].Success)
            {
                var tagname = match.Groups["tagname"].Value;
                BlazorNode activeNode = new BlazorNode(){Name = tagname};
                if(!tags.ContainsKey(tagname))
                {
                    tags.Add(tagname, tagname);
                }
                //If there is no root, first tag becomes root
                if(root == null)
                {
                    root = activeNode;
                    currentNode = root;
                }
                //Handle self closing tag
                if(match.Groups["selfclosingtag"].Success || (match.Groups[""].Success && match.Groups[""].Value.EndsWith("\\")))
                //Regex doesn't handle the case of an unquoted attribute right before the end of a self closing tag (ie ...lastattribute=value/>), so we do an explicit check
                {
                    currentNode.IsOpen = false;
                    if (currentNode?.Parent == null)
                    {
                        //end evaluation if we hit root
                        return;
                    }
                    else
                    {
                        //Move up a level on the tree
                        currentNode = currentNode.Parent;
                    }
                    continue;
                }
                //Tag isn't self closing, handle moving down tree
                if(currentNode == root)
                {
                    continue;
                }
                currentNode.Children.Add(activeNode);
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