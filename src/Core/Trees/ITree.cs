namespace Ocluse.LiquidSnow.Trees;

/// <summary>
/// A contract for a tree data structure.
/// </summary>
/// <typeparam name="T">The type of data on each tree node</typeparam>
public interface ITree<T>
{
    /// <summary>
    /// The data contained in the tree node.
    /// </summary>
    T? Data { get; }

    /// <summary>
    /// The parent of the tree node.
    /// </summary>
    ITree<T>? Parent { get; }

    /// <summary>
    /// The children of the tree node.
    /// </summary>
    ICollection<ITree<T>> Children { get; }
    
    /// <summary>
    /// Returns true if the tree node is the root of the tree.
    /// </summary>
    bool IsRoot { get; }

    /// <summary>
    /// Returns true if the tree node is a leaf of the tree.
    /// </summary>
    bool IsLeaf { get; }

    /// <summary>
    /// Returns the level of the tree node, with the root node being level 0.
    /// </summary>
    int Level { get; }
}
