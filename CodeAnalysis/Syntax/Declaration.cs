using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class Declaration(DeclarationKind DeclarationKind) : Statement(SyntaxNodeKind.DeclarationStatement)
{
    public override TextSpan Span { get => TextSpan.FromBounds(GetChildren().First().Span.Start, GetChildren().Last().Span.End); }
}
