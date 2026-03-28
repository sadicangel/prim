using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ExpressionSyntax ParseExpressionTerminated(SyntaxIterator iterator) =>
        ParseExpression(iterator, allowUnterminated: false);
}
