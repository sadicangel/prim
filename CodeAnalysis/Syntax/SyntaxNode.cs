using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class SyntaxNode(SyntaxNodeKind NodeKind) : INode
{
    public abstract TextSpan Span { get; }

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        writer.WriteColored(indent, ConsoleColor.DarkGray);
        writer.WriteColored(marker, ConsoleColor.DarkGray);
        writer.WriteColored(NodeKind, ConsoleColor.Cyan);
        if (this is Token { Value: not null } token)
        {
            writer.Write(' ');
            writer.WriteColored(token.Value, ConsoleColor.DarkGreen);
        }
        writer.WriteLine();

        indent += isLast ? "   " : "│  ";

        var lastChild = GetChildren().LastOrDefault();

        foreach (var child in GetChildren())
            child.WriteTo(writer, indent, child == lastChild);
    }

    public abstract IEnumerable<SyntaxNode> GetChildren();

    IEnumerable<INode> INode.GetChildren() => GetChildren();

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}