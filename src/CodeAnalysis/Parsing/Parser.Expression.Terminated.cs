using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ExpressionSyntax ParseExpressionTerminated(SyntaxTokenStream stream) =>
        ParseExpression(stream, allowUnterminated: false);
}
