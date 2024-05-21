using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static FunctionDeclarationSyntax ParseFunctionDeclaration(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var type = ParseType(syntaxTree, iterator);
        if (type is not FunctionTypeSyntax functionType)
            throw new InvalidOperationException($"Unexpected type '{type.GetType().Name}'. Expected '{nameof(FunctionTypeSyntax)}'");
        var operatorToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
        var expression = ParseExpression(syntaxTree, iterator, isTerminated: true);

        return new FunctionDeclarationSyntax(
            syntaxTree,
            identifierToken,
            colonToken,
            functionType,
            operatorToken,
            expression);
    }
}
