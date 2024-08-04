using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Expressions.Declarations;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static VariableDeclarationSyntax ParseVariableDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        // name: type = value;
        // name:= value;
        // name: type;
        var name = ParseSimpleName(syntaxTree, iterator);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var type = default(TypeSyntax);
        var initValue = default(InitValueExpressionSyntax);
        var semicolonToken = default(SyntaxToken);
        if (iterator.TryMatch(out var equalsToken, SyntaxKind.EqualsToken))
        {
            var expression = ParseExpression(syntaxTree, iterator);
            initValue = new InitValueExpressionSyntax(syntaxTree, equalsToken, expression);
        }
        else
        {
            type = ParseType(syntaxTree, iterator);
            if (iterator.TryMatch(out equalsToken, SyntaxKind.EqualsToken))
            {
                var expression = ParseExpression(syntaxTree, iterator);
                initValue = new InitValueExpressionSyntax(syntaxTree, equalsToken, expression);
            }
        }
        if (initValue?.IsTerminated is not true)
            semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);

        return new VariableDeclarationSyntax(
            syntaxTree,
            name,
            colonToken,
            type,
            initValue,
            semicolonToken);
    }
}
