using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ContinueExpressionSyntax ParseContinueExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var continueKeyword = iterator.Match(SyntaxKind.ContinueKeyword);
        var expression = default(ExpressionSyntax);
        if (!iterator.TryMatch(out var semicolonToken, SyntaxKind.SemicolonToken))
        {
            expression = ParseExpression(syntaxTree, iterator);
            if (!expression.IsTerminated)
                semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);
        }
        return new ContinueExpressionSyntax(syntaxTree, continueKeyword, expression, semicolonToken);
    }
}
