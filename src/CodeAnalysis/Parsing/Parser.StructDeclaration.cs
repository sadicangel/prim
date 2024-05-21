using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static StructDeclarationSyntax ParseStructDeclaration(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var typeToken = iterator.Match(SyntaxKind.TypeKeyword);
        var operatorToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
        var expression = ParseExpression(syntaxTree, iterator, isTerminated: true);

        return new StructDeclarationSyntax(
            syntaxTree,
            identifierToken,
            colonToken,
            typeToken,
            operatorToken,
            expression);
    }
}
