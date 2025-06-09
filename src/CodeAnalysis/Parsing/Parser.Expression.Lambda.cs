using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    public static LambdaExpressionSyntax ParseLambdaExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var lambdaKeyword = iterator.Match(SyntaxKind.LambdaKeyword);
        var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
        var parameters = ParseSyntaxList(
            syntaxTree,
            iterator,
            SyntaxKind.CommaToken,
            [SyntaxKind.ParenthesisCloseToken],
            static (syntaxTree, iterator) =>
            {
                var name = ParseSimpleName(syntaxTree, iterator);
                var typeClause = ParseTypeClause(syntaxTree, iterator, isOptional: true);

                return new ParameterSyntax(syntaxTree, name, typeClause);
            });
        var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
        var body = iterator.TryMatch(out var arrowLambdaToken, SyntaxKind.ArrowLambdaToken)
            ? ParseStatementExpression(syntaxTree, iterator)
            : ParseBlockExpression(syntaxTree, iterator) as ExpressionSyntax;

        return new LambdaExpressionSyntax(
            syntaxTree,
            lambdaKeyword,
            parenthesisOpenToken,
            parameters,
            parenthesisCloseToken,
            arrowLambdaToken,
            body);
    }
}
