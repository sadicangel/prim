using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static StatementExpressionSyntax ParseStatementExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var expression = ParseExpression(syntaxTree, iterator, isTerminated: false);
        var semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);
        return new StatementExpressionSyntax(syntaxTree, expression, semicolonToken);
    }
}
