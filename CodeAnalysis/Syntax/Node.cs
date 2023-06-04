using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class Node(NodeKind Kind) : INode
{
    public TextSpan Span { get => GetChildren().Last().Span - GetChildren().First().Span; }

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        writer.WriteColored(indent, ConsoleColor.DarkGray);
        writer.WriteColored(marker, ConsoleColor.DarkGray);
        writer.WriteColored(Kind, ConsoleColor.Cyan);
        writer.WriteLine();

        indent += isLast ? "   " : "│  ";

        var lastChild = GetChildren().LastOrDefault();

        foreach (var child in GetChildren())
            child.WriteTo(writer, indent, child == lastChild);
    }

    public abstract IEnumerable<INode> GetChildren();

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}