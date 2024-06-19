using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static EmptyExpressionSyntax ParseEmptyExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);
        return new EmptyExpressionSyntax(syntaxTree, semicolonToken);
    }
}
