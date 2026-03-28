using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static EmptyExpressionSyntax ParseEmptyExpression(SyntaxIterator iterator)
    {
        var semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);

        return new EmptyExpressionSyntax(semicolonToken);
    }
}
