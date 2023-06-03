namespace CodeAnalysis.Syntax;

public sealed record class Token(TokenKind Kind, int Position, string Text, object? Value = null) : INode
{
    public TextSpan Span { get => new(Position, Text.Length); }

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
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
            child.WriteTo(writer, indent, child == lastChild);
    }

    public IEnumerable<INode> GetChildren() => Enumerable.Empty<INode>();

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}