namespace Ocluse.LiquidSnow.Nodes;

internal class Node<T> : INode<T>
{
    public T? Data { get; }
    public INode<T>? Parent { get; private set; }
    public ICollection<INode<T>> Children { get; }
    public bool IsRoot => Parent == null;
    public bool IsLeaf => Children.Count == 0;
    public int Level => IsRoot ? 0 : Parent!.Level + 1;
    private Node(T? data)
    {
        Children = new LinkedList<INode<T>>();
        Data = data;
    }
    public static Node<T> FromLookup(ILookup<T?, T> lookup)
    {
        var rootData = lookup.Count == 1 ? lookup.First().Key : default;
        var root = new Node<T>(rootData);
        root.LoadChildren(lookup);
        return root;
    }
    private void LoadChildren(ILookup<T?, T> lookup)
    {
        foreach (var data in lookup[Data])
        {
            var child = new Node<T>(data) { Parent = this };
            Children.Add(child);
            child.LoadChildren(lookup);
        }
    }
}
