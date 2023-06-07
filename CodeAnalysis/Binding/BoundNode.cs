using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using System.Collections.Concurrent;
using System.Reflection;

namespace CodeAnalysis.Binding;

internal abstract record class BoundNode(BoundNodeKind Kind) : INode
{
    public TextSpan Span { get => throw new NotSupportedException(); }

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
            BoundBinaryExpression e => ($"{e.Operator.Kind}Expression", ConsoleColor.Blue),
            BoundUnaryExpression e => ($"{e.Operator.Kind}Expression", ConsoleColor.Blue),
            BoundExpression e => (e.Kind.ToString(), ConsoleColor.Blue),
            BoundStatement s => (s.Kind.ToString(), ConsoleColor.Cyan),
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

                writer.WriteColored(property.Name, ConsoleColor.DarkYellow);
                writer.WriteColored(" = ", ConsoleColor.DarkGray);
                writer.WriteColored(property.Value, ConsoleColor.Yellow);
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

file readonly record struct NodeProperty(string Name, object Value);

file static class NodePropertyCache
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<NodeProperty>> Cache = new();

    public static IReadOnlyList<NodeProperty> GetProperties(INode node)
    {
        var type = node.GetType();
        if (!Cache.TryGetValue(type, out var properties))
        {
            var list = new List<NodeProperty>();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.Name is nameof(BoundNode.Kind) or nameof(BoundNode.Span))
                    continue;

                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType) || typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                    continue;

                var value = property.GetValue(node);
                if (value is not null)
                    list.Add(new NodeProperty(property.Name, value));
            }
            Cache[type] = properties = list;
        }
        return properties;
    }
}