using CodeAnalysis.Syntax.Names;
using CodeAnalysis.Syntax.Statements;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class LambdaExpressionSyntax(
    SyntaxToken ParenthesisOpenToken,
    SeparatedSyntaxList<SimpleNameSyntax> Parameters,
    SyntaxToken ParenthesisCloseToken,
    SyntaxToken EqualsGreaterThanToken,
    StatementSyntax Body)
    : ExpressionSyntax(SyntaxKind.LambdaExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ParenthesisOpenToken;
        foreach (var parameter in Parameters.SyntaxNodes)
            yield return parameter;
        yield return ParenthesisCloseToken;
        yield return EqualsGreaterThanToken;
        yield return Body;
    }
}
