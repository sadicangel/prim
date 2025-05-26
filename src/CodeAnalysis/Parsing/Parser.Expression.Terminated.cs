using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParseExpressionTerminated(SyntaxTree syntaxTree, SyntaxIterator iterator) =>
        ParseExpression(syntaxTree, iterator, allowUnterminated: false);
}
