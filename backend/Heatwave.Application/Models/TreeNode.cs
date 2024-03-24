namespace Heatwave.Application.Models;
public interface INode<TData>
{
    long Id { get; }
    long? ParentId { get; }
    string Title { get; }
    int Rank { get; }
    ICollection<TData> Children { get; set; }
}

public static class TreeNodeExtensions
{
    public static ICollection<TNode> Build<TNode>(this IEnumerable<TNode>? nodes)
        where TNode : INode<TNode>
    {
        if (nodes is null || !nodes.Any())
            return new List<TNode>();

        var pids = nodes.Where(v => v.ParentId.HasValue).Select(v => v.ParentId!.Value).ToList();
        if (pids is null)
            return new List<TNode>();
        var ids = nodes.Where(v => v.ParentId is not null).Select(v => v.Id).ToList();

        var pNodes = nodes.Where(v => !v.ParentId.HasValue || pids.Contains(v.Id)).ToList();

        foreach (var node in nodes)
        {
            var n = nodes.FirstOrDefault(v => v.Id == node.ParentId);
            if (n is not null)
            {
                if (n.Children is null)
                {
                    n.Children = new List<TNode>();
                }

                n.Children.Add(node);
            }
        }

        return pNodes;
    }
}