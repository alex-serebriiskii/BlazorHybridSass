namespace BlazorHybridSass;

class BlazorNode
{
    public BlazorNode? Parent {get; set;}
    public List<BlazorNode> Children {get; set;} = new List<BlazorNode>();
    public string Name {get; set;}= "";
    public bool IsOpen {get; set;} = true;
}