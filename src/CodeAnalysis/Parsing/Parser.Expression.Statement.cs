using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static StatementExpressionSyntax ParseStatementExpression(SyntaxTokenStream stream)
    {
        var expression = ParseBinaryExpression(stream);
        var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);

        return new StatementExpressionSyntax(expression, semicolonToken);
    }
}
