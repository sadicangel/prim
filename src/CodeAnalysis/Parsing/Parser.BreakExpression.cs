using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static BreakExpressionSyntax ParseBreakExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var breakKeyword = iterator.Match(SyntaxKind.BreakKeyword);
        var expression = default(ExpressionSyntax);
        if (!iterator.TryMatch(out var semicolonToken, SyntaxKind.SemicolonToken))
        {
            expression = ParseExpression(syntaxTree, iterator);
            if (!expression.IsTerminated)
                semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);
        }
        return new BreakExpressionSyntax(syntaxTree, breakKeyword, expression, semicolonToken);
    }
}
