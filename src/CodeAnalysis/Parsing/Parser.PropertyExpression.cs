using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static PropertyExpressionSyntax ParsePropertyExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var dotToken = iterator.Match(SyntaxKind.DotToken);
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
        var equalsToken = iterator.Match(SyntaxKind.EqualsToken);
        var init = ParseExpression(syntaxTree, iterator);
        return new PropertyExpressionSyntax(syntaxTree, dotToken, identifierToken, equalsToken, init);
    }
}
