using CodeAnalysis.Syntax;
using System.Collections.Concurrent;
using System.Reflection;
using LinqExpr = System.Linq.Expressions.Expression;

namespace CodeAnalysis.Binding;

internal abstract record class BoundNode(BoundNodeKind Kind) : INode
{
    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        writer.WriteColored(indent, ConsoleColor.DarkGray);
        writer.WriteColored(marker, ConsoleColor.DarkGray);
        var (text, color) = GetTextAndColor(this);
        writer.WriteColored(text, color);
        WriteProperties(writer, this);
        writer.WriteLine();

        indent += isLast ? "   " : "│  ";

        var lastChild = GetChildren().LastOrDefault();

        foreach (var child in GetChildren())
            child.WriteTo(writer, indent, child == lastChild);

        static (string Text, ConsoleColor Color) GetTextAndColor(INode node) => node switch
        {
            BoundBinaryExpression e => ($"{e.Operator.Kind}Expression ", ConsoleColor.Blue),
            BoundUnaryExpression e => ($"{e.Operator.Kind}Expression ", ConsoleColor.Blue),
            BoundExpression e => ($"{e.Kind} ", ConsoleColor.Blue),
            BoundStatement s => ($"{s.Kind} ", ConsoleColor.Cyan),
            _ => throw new InvalidOperationException($"Invalid node type '{node?.GetType()}' in bound tree")
        };

        static void WriteProperties(TextWriter writer, INode node)
        {
            var isFirst = true;
            foreach (var property in NodePropertyCache.GetProperties(node))
            {
                if (isFirst)
                    isFirst = false;
                else
                    writer.WriteColored(", ", ConsoleColor.DarkGray);

                writer.WriteColored(property.Name, ConsoleColor.Yellow);
                writer.WriteColored(" = ", ConsoleColor.DarkGray);
                writer.WriteColored(property.GetValue(node), ConsoleColor.DarkYellow);
            }
        };
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
                if (property.Name is nameof(Node.Span) or nameof(BoundNode.Kind) or nameof(BoundBinaryExpression.Operator))
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