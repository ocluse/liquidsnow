using Ocluse.LiquidSnow.Nodes;

namespace Ocluse.LiquidSnow.Extensions;

/// <summary>
/// Extensions for tree data structure.
/// </summary>
public static class NodeExtensions
{
    /// <summary>
    /// Flattens the node structure.
    /// </summary>
    public static IEnumerable<TNode> Flatten<TNode>(this IEnumerable<TNode> nodes, Func<TNode, IEnumerable<TNode>> childrenSelector)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        return nodes.SelectMany(c => childrenSelector(c).Flatten(childrenSelector)).Concat(nodes);
    }

    /// <summary>
    /// Converts a collection of items to a node structure using the provided parent selector.
    /// </summary>
    /// <remarks>
    /// The first argument of the parent selector is the test parent item, the second argument is the child item.
    /// </remarks>
    public static INode<T> ToNodeTree<T>(this IEnumerable<T> items, Func<T, T, bool> parentSelector)
    {
        ArgumentNullException.ThrowIfNull(items);
        var lookup = items.ToLookup(item => items.FirstOrDefault(parent => parentSelector(parent, item)),
            child => child);
        return Node<T>.FromLookup(lookup);
    }
}
