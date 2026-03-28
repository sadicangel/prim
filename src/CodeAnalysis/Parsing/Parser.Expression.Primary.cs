using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // primary_expression = type | literal | group | local_declaration | name
    private static ExpressionSyntax ParsePrimaryExpression(SyntaxIterator iterator)
    {
        var current = iterator.Current;
        return current.SyntaxKind switch
        {
            >= SyntaxKind.AnyKeyword and <= SyntaxKind.F64Keyword => new SimpleNameSyntax(iterator.Match()),
            >= SyntaxKind.TrueKeyword and <= SyntaxKind.NullKeyword => ParseLiteralExpression(iterator),
            >= SyntaxKind.I8LiteralToken and <= SyntaxKind.StrLiteralToken => ParseLiteralExpression(iterator),
            SyntaxKind.ParenthesisOpenToken when iterator.IsLambdaExpressionAhead() => ParseLambdaExpression(iterator),
            SyntaxKind.ParenthesisOpenToken => ParseGroupExpression(iterator),
            //_ when iterator.Peek(1).SyntaxKind is SyntaxKind.ColonToken => ParseLocalDeclaration(iterator),
            _ => ParseName(iterator),
        };
    }
}
