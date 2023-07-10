using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Expressions;

public abstract record class Expression(SyntaxNodeKind NodeKind, SyntaxTree SyntaxTree) : SyntaxNode(NodeKind, SyntaxTree)
{
    public override TextSpan Span { get => TextSpan.FromBounds(GetChildren().First().Span.Start, GetChildren().Last().Span.End); }

    public abstract T Accept<T>(ISyntaxExpressionVisitor<T> visitor);
}