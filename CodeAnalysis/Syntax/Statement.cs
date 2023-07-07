using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class Statement(SyntaxNodeKind NodeKind, SyntaxTree SyntaxTree) : SyntaxNode(NodeKind, SyntaxTree)
{
    public override TextSpan Span { get => TextSpan.FromBounds(GetChildren().First().Span.Start, GetChildren().Last().Span.End); }

    public abstract T Accept<T>(ISyntaxStatementVisitor<T> visitor);
}