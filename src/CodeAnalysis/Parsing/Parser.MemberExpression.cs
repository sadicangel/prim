using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static MemberExpressionSyntax ParseMemberExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var dotToken = iterator.Match(SyntaxKind.DotToken);
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
        var equalsToken = iterator.Match(SyntaxKind.EqualsToken);
        var expression = ParseExpression(syntaxTree, iterator);
        return new MemberExpressionSyntax(syntaxTree, dotToken, identifierToken, equalsToken, expression);
    }
}
