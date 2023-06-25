using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class Expression(SyntaxNodeKind Kind) : SyntaxNode(Kind)
{
    public override TextSpan Span { get => TextSpan.FromBounds(GetChildren().First().Span.Start, GetChildren().Last().Span.End); }

    public abstract T Accept<T>(ISyntaxExpressionVisitor<T> visitor);
}