using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Statements;

public sealed record class BreakStatementSyntax(SyntaxToken BreakKeyword, ExpressionSyntax? Expression, SyntaxToken SemicolonToken)
    : StatementSyntax(SyntaxKind.BreakStatement)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BreakKeyword;
        if (Expression is not null) yield return Expression;
        yield return SemicolonToken;
    }
}
