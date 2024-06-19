using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static StatementExpressionSyntax ParseStatementExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var expression = ParseExpression(syntaxTree, iterator);
        var semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);
        return new StatementExpressionSyntax(syntaxTree, expression, semicolonToken);
    }
}
