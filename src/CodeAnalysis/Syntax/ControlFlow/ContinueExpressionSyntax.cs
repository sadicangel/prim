using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.ControlFlow;

public sealed record class ContinueExpressionSyntax(SyntaxToken ContinueKeyword, SyntaxToken SemicolonToken)
    : ExpressionSyntax(SyntaxKind.ContinueExpression)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ContinueKeyword;
        yield return SemicolonToken;
    }
}
