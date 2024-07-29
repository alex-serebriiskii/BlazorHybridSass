namespace BlazorHybridSass;

class BlazorNode
{
    public BlazorNode? Parent {get; set;}
    public BlazorNode? DependentRoot{get;set;}
    public List<BlazorNode> Children {get; set;} = new List<BlazorNode>();
    public string Name {get; set;}= "";
    public bool IsOpen {get; set;} = true;
    public bool IsRoot {get; set;} = false;
    public bool IsDependent {get; set;} = false;

    public void BindDependents(Dictionary<string, BlazorNode> roots)
    {
        //avoid creating a reference to itself if it's the root element
        if(roots.ContainsKey(Name) && !IsRoot)
        {
            DependentRoot = roots[Name];
            roots[Name].IsDependent = true;
        }
        foreach(var child in Children)
        {
            child.BindDependents(roots);
        }
    }

}