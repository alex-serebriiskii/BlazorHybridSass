namespace BlazorHybridSass;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        DirectoryInfo d = new DirectoryInfo("");
        var testDir = d.GetFiles("*.razor*", SearchOption.AllDirectories);
        var testfile = testDir[0];
        BlazorTree tree = new BlazorTree();
        tree.BuildTree(testfile);
    }
}
