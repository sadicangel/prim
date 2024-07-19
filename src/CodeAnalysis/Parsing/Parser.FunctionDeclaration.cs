using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static FunctionDeclarationSyntax ParseFunctionDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var name = ParseIdentifierNameExpression(syntaxTree, iterator);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var type = ParseType(syntaxTree, iterator);
        if (type is not LambdaTypeSyntax functionType)
            throw new InvalidOperationException($"Unexpected type '{type.GetType().Name}'. Expected '{nameof(LambdaTypeSyntax)}'");
        var colonOrEqualsToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
        var body = ParseTerminatedExpression(syntaxTree, iterator);

        return new FunctionDeclarationSyntax(
            syntaxTree,
            name,
            colonToken,
            functionType,
            colonOrEqualsToken,
            body);
    }
}
