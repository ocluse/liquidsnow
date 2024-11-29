namespace Ocluse.LiquidSnow.Nodes;

/// <summary>
/// Defines a node.
/// </summary>
/// <typeparam name="T">The type of data on each node</typeparam>
public interface INode<T>
{
    /// <summary>
    /// Gets the data contained in the node.
    /// </summary>
    T? Data { get; }

    /// <summary>
    /// Gets the parent of the node, or null if its the root.
    /// </summary>
    INode<T>? Parent { get; }

    /// <summary>
    /// Gets the children of the node.
    /// </summary>
    ICollection<INode<T>> Children { get; }

    /// <summary>
    /// Gets a value indicating whether the node is the root of the tree.
    /// </summary>
    bool IsRoot { get; }

    /// <summary>
    /// Gets a value indicating whether the node is a leaf node (has no children).
    /// </summary>
    bool IsLeaf { get; }

    /// <summary>
    /// Gets a value indicating the depth level of the node from root, with the root node being level 0.
    /// </summary>
    int Level { get; }
}
