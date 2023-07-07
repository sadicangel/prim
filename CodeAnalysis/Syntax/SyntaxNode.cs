using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class SyntaxNode(SyntaxNodeKind NodeKind, SyntaxTree SyntaxTree) : INode
{
    public abstract TextSpan Span { get; }

    public Token GetLastToken()
    {
        if (this is Token token)
            return token;

        return GetChildren().Last().GetLastToken();
    }

    public TextLocation GetLocation() => new(SyntaxTree.Text, Span);

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        writer.WriteColored(indent, ConsoleColor.DarkGray);
        writer.WriteColored(marker, ConsoleColor.DarkGray);
        switch (this)
        {
            case Token token:
                writer.WriteColored($"{token.TokenKind}Token", ConsoleColor.Blue);
                if (token.Value is not null)
                {
                    writer.Write(' ');
                    writer.WriteColored(token.Value, ConsoleColor.DarkGreen);
                }
                break;
            default:
                writer.WriteColored(NodeKind, ConsoleColor.Cyan);
                break;
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