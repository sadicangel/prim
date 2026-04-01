using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ExpressionSyntax ParseBlockExpressionStatement(SyntaxTokenStream stream) => stream.Current.SyntaxKind switch
    {
        SyntaxKind.LetKeyword or SyntaxKind.VarKeyword => ParseVariableDeclaration(stream),
        _ => ParseExpressionTerminated(stream),
    };
}
