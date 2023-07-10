using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using System.Collections.Concurrent;
using System.Reflection;
using LinqExpr = System.Linq.Expressions.Expression;

namespace CodeAnalysis.Binding;

internal abstract record class BoundNode(BoundNodeKind NodeKind) : INode
{
    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
    {
        BoundNodeWriterExtensions.WriteTo(this, writer);
    }

    public abstract IEnumerable<INode> GetChildren();

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}

file readonly record struct NodePropertyAccessor(string Name, Func<INode, object> GetValue);

file static class NodePropertyCache
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<NodePropertyAccessor>> Cache = new();

    public static IReadOnlyList<NodePropertyAccessor> GetProperties(INode node)
    {
        var type = node.GetType();
        if (!Cache.TryGetValue(type, out var properties))
        {
            var list = new List<NodePropertyAccessor>();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Reverse())
            {
                if (property.Name is nameof(SyntaxNode.Span) or nameof(BoundNode.NodeKind) or nameof(BoundBinaryExpression.Operator))
                    continue;

                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType) || typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                    continue;

                var parameter = LinqExpr.Parameter(typeof(INode), "node");
                var body = LinqExpr.Convert(LinqExpr.Property(LinqExpr.Convert(parameter, type), property), typeof(object));
                var func = LinqExpr.Lambda<Func<INode, object>>(body, parameter).Compile();

                var value = property.GetValue(node);
                if (value is not null)
                    list.Add(new NodePropertyAccessor(property.Name, func));
            }
            Cache[type] = properties = list;
        }
        return properties;
    }
}