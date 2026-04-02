namespace CodeAnalysis.Syntax.Expressions;

public sealed record class ElementAccessExpressionSyntax(
    ExpressionSyntax Receiver,
    SyntaxToken BracketOpen,
    ExpressionSyntax Index,
    SyntaxToken BracketClose)
    : ExpressionSyntax(SyntaxKind.ElementAccessExpression)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Receiver;
        yield return BracketOpen;
        yield return Index;
        yield return BracketClose;
    }
}
