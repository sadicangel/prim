using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class SyntaxNode(SyntaxNodeKind NodeKind, SyntaxTree SyntaxTree) : INode
{
    public virtual TextSpan Span { get => TextSpan.FromBounds(Children().First().Span.Start, Children().Last().Span.End); }
    public virtual TextSpan FullSpan { get => TextSpan.FromBounds(Children().First().FullSpan.Start, Children().Last().FullSpan.End); }

    public SyntaxNode? Parent { get => SyntaxTree.GetParent(this); }

    public Token GetLastToken()
    {
        if (this is Token token)
            return token;

        return Children().Last().GetLastToken();
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

        var lastChild = Children().LastOrDefault();

        foreach (var child in Children())
            child.WriteTo(writer, indent, child == lastChild);
    }

    public abstract IEnumerable<SyntaxNode> Children();

    IEnumerable<INode> INode.Children() => Children();

    public IEnumerable<SyntaxNode> DescendantsAndSelf()
    {
        yield return this;

        var queue = new Queue<SyntaxNode>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            foreach (var child in node.Children())
            {
                yield return child;
                queue.Enqueue(child);
            }
        }
    }

    public IEnumerable<SyntaxNode> Descendants() => DescendantsAndSelf().Skip(1);

    public IEnumerable<SyntaxNode> AncestorsAndSelf()
    {
        var node = this;
        while (node is not null)
        {
            yield return node;
            node = node.Parent;
        }
    }
    public IEnumerable<SyntaxNode> Ancestors() => AncestorsAndSelf().Skip(1);

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}