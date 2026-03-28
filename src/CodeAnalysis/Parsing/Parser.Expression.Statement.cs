using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static StatementExpressionSyntax ParseStatementExpression(SyntaxIterator iterator)
    {
        var expression = ParseBinaryExpression(iterator);
        var semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);

        return new StatementExpressionSyntax(expression, semicolonToken);
    }
}
