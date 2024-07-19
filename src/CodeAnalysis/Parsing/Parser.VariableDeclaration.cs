using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static VariableDeclarationSyntax ParseVariableDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var name = ParseIdentifierNameExpression(syntaxTree, iterator);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var type = default(TypeSyntax);
        if (!iterator.TryMatch(out var colonOrEqualsToken, SyntaxKind.EqualsToken, SyntaxKind.ColonToken))
        {
            type = ParseType(syntaxTree, iterator); ;
            colonOrEqualsToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
        }
        var expression = ParseTerminatedExpression(syntaxTree, iterator);

        return new VariableDeclarationSyntax(
            syntaxTree,
            name,
            colonToken,
            type,
            colonOrEqualsToken,
            expression);
    }
}
