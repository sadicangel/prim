namespace CodeAnalysis.Syntax;

public sealed record class Token(TokenKind Kind, int Position, string Text, object? Value = null) : IPrintableNode
{
    public TextSpan Span { get => new TextSpan(Position, Text.Length); }

    public void PrettyPrint(TextWriter writer, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        writer.Write(indent);
        writer.Write(marker);
        writer.Write(Kind);
        if (Value is not null)
        {
            writer.Write(' ');
            writer.Write(Value);
        }
        writer.WriteLine();

        indent += isLast ? "   " : "│  ";

        var lastChild = GetChildren().LastOrDefault();

        foreach (var child in GetChildren())
            child.PrettyPrint(writer, indent, child == lastChild);
    }

    public IEnumerable<IPrintableNode> GetChildren() => Enumerable.Empty<IPrintableNode>();

}