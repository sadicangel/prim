using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.ControlFlow;

public sealed record class ReturnExpressionSyntax(SyntaxToken ReturnKeyword, ExpressionSyntax? Expression, SyntaxToken? SemicolonToken)
    : ExpressionSyntax(SyntaxKind.ReturnExpression)
{
    public ReturnExpressionSyntax(SyntaxToken ReturnKeyword, ExpressionSyntax Expression) : this(ReturnKeyword, Expression, null) { }

    public ReturnExpressionSyntax(SyntaxToken ReturnKeyword, SyntaxToken SemicolonToken) : this(ReturnKeyword, null, SemicolonToken) { }

    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ReturnKeyword;
        if (Expression is not null) yield return Expression;
        if (SemicolonToken is not null) yield return SemicolonToken;
    }
}
