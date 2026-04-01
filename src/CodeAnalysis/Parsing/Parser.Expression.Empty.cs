using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static EmptyExpressionSyntax ParseEmptyExpression(SyntaxTokenStream stream)
    {
        var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);

        return new EmptyExpressionSyntax(semicolonToken);
    }
}
