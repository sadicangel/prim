using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ExpressionSyntax ParseBlockExpressionStatement(SyntaxIterator iterator) => iterator.Current.SyntaxKind switch
    {
        SyntaxKind.LetKeyword or SyntaxKind.VarKeyword => ParseVariableDeclaration(iterator),
        _ => ParseExpressionTerminated(iterator),
    };
}
