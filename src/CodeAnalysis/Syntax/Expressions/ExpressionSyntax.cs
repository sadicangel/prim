namespace CodeAnalysis.Syntax.Expressions;

public abstract record class ExpressionSyntax(SyntaxKind SyntaxKind)
    : SyntaxNode(SyntaxKind)
{
    public bool IsTerminated { get => LastToken.SyntaxKind is SyntaxKind.SemicolonToken or SyntaxKind.BraceCloseToken; }
}
