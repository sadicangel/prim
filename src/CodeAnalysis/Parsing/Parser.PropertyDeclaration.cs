using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static PropertyDeclarationSyntax ParsePropertyDeclaration(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var type = ParseType(syntaxTree, iterator);
        var operatorToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
        var expression = ParseTerminatedExpression(syntaxTree, iterator);

        return new PropertyDeclarationSyntax(
            syntaxTree,
            identifierToken,
            colonToken,
            type,
            operatorToken,
            expression);
    }
}
