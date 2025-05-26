namespace CodeAnalysis.Syntax.Expressions;
public abstract record class ExpressionSyntax(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
    : SyntaxNode(SyntaxKind, SyntaxTree)
{
    public bool IsTerminated { get => LastToken.SyntaxKind is SyntaxKind.SemicolonToken or SyntaxKind.BraceCloseToken; }
}
