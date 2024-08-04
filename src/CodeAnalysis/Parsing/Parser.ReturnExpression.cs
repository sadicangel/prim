using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Expressions.ControlFlow;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ReturnExpressionSyntax ParseReturnExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var returnKeyword = iterator.Match(SyntaxKind.ReturnKeyword);
        var expression = default(ExpressionSyntax);
        if (!iterator.TryMatch(out var semicolonToken, SyntaxKind.SemicolonToken))
        {
            expression = ParseExpression(syntaxTree, iterator);
            if (!expression.IsTerminated)
                semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);
        }
        return new ReturnExpressionSyntax(syntaxTree, returnKeyword, expression, semicolonToken);
    }
}
