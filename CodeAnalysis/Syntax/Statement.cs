using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class Statement(NodeKind Kind) : Node(Kind)
{
    public override TextSpan Span { get => TextSpan.FromBounds(GetChildren().First().Span.Start, GetChildren().Last().Span.End); }

    public abstract T Accept<T>(IStatementVisitor<T> visitor);
}