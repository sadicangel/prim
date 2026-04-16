using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Statements;

public sealed record class ReturnStatementSyntax(SyntaxToken ReturnKeyword, ExpressionSyntax? Expression, SyntaxToken SemicolonToken)
    : StatementSyntax(SyntaxKind.ReturnStatement)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ReturnKeyword;
        if (Expression is not null) yield return Expression;
        yield return SemicolonToken;
    }
}
