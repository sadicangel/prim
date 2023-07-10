using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax.Statements;

public abstract record class Declaration(SyntaxTree SyntaxTree)
    : Statement(SyntaxNodeKind.DeclarationStatement, SyntaxTree)
{
    public override TextSpan Span { get => TextSpan.FromBounds(GetChildren().First().Span.Start, GetChildren().Last().Span.End); }
}
