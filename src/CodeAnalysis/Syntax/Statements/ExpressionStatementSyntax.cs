using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Statements;

public sealed record class ExpressionStatementSyntax(ExpressionSyntax Expression, SyntaxToken SemicolonToken)
    : StatementSyntax(SyntaxKind.ExpressionStatement)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return SemicolonToken;
    }
}
