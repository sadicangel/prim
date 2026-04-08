using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.ControlFlow;

public sealed record class BreakExpressionSyntax(SyntaxToken BreakKeyword, ExpressionSyntax? Expression, SyntaxToken? SemicolonToken)
    : ExpressionSyntax(SyntaxKind.BreakExpression)
{
    public BreakExpressionSyntax(SyntaxToken BreakKeyword, ExpressionSyntax Expression) : this(BreakKeyword, Expression, null) { }

    public BreakExpressionSyntax(SyntaxToken BreakKeyword, SyntaxToken SemicolonToken) : this(BreakKeyword, null, SemicolonToken) { }

    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BreakKeyword;
        if (Expression is not null) yield return Expression;
        if (SemicolonToken is not null) yield return SemicolonToken;
    }
}
