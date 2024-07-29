using System.IO;

namespace BlazorHybridSass;

class BlazorTreeBuilder
{
    private string _blazorDirPath;
    private Dictionary<string, BlazorNode> roots = new Dictionary<string, BlazorNode>();
    private List<BlazorTree> trees = new List<BlazorTree>();
    private BlazorTreeBuilder(string blazorDirPath)
    {
        _blazorDirPath = blazorDirPath;
    }
    private void BuildTrees()
    {
       DirectoryInfo dir = new DirectoryInfo(_blazorDirPath);
       var files = dir.GetFiles("*.razor", SearchOption.AllDirectories);  
       foreach(var file in files)
       {
            var tree = new BlazorTree(file);
            trees.Add(tree);
            roots.Add(tree.Root.Name,tree.Root);
       }
       foreach(var root in roots.Values)
       {
            root.BindDependents(roots);
       }
    }
}