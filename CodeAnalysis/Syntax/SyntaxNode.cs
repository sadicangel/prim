using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class SyntaxNode(SyntaxNodeKind NodeKind, SyntaxTree SyntaxTree) : INode
{
    public virtual TextSpan Span { get => TextSpan.FromBounds(GetChildren().First().Span.Start, GetChildren().Last().Span.End); }
    public virtual TextSpan FullSpan { get => TextSpan.FromBounds(GetChildren().First().FullSpan.Start, GetChildren().Last().FullSpan.End); }

    public Token GetLastToken()
    {
        if (this is Token token)
            return token;

        return GetChildren().Last().GetLastToken();
    }

    public TextLocation GetLocation() => new(SyntaxTree.Text, Span);

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
    {
        var token = this as Token;

        if (token is not null)
        {
            foreach (var trivia in token.LeadingTrivia)
            {
                writer.WriteColored(indent, ConsoleColor.DarkGray);
                writer.WriteColored("├──", ConsoleColor.DarkGray);
                writer.WriteLineColored($"L: {trivia.TokenKind}", ConsoleColor.DarkGreen);
            }
        }

        var hasTrailingTrivia = token?.TrailingTrivia?.Count > 0;

        var marker = !hasTrailingTrivia && isLast ? "└──" : "├──";

        writer.WriteColored(indent, ConsoleColor.DarkGray);
        writer.WriteColored(marker, ConsoleColor.DarkGray);

        if (token is not null)
        {
            writer.WriteColored($"{token.TokenKind}Token", ConsoleColor.Blue);
            if (token.Value is not null)
            {
                writer.Write(' ');
                writer.WriteColored(token.Value, ConsoleColor.DarkGreen);
            }
        }
        else
        {
            writer.WriteColored(NodeKind, ConsoleColor.Cyan);
        }
        writer.WriteLine();

        if (token is not null)
        {
            foreach (var trivia in token.TrailingTrivia)
            {
                writer.WriteColored(indent, ConsoleColor.DarkGray);
                writer.WriteColored(isLast && trivia == token.TrailingTrivia[^1] ? "└──" : "├──", ConsoleColor.DarkGray);
                writer.WriteLineColored($"T: {trivia.TokenKind}", ConsoleColor.DarkGreen);
            }
        }

        indent += isLast ? "   " : "│  ";

        var lastChild = GetChildren().LastOrDefault();

        foreach (var child in GetChildren())
            child.WriteTo(writer, indent, child == lastChild);
    }

    public abstract IEnumerable<SyntaxNode> GetChildren();

    IEnumerable<INode> INode.Descendants() => GetChildren();

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}