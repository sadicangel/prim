using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // primary_expression = type | literal | group | local_declaration | name
    private static ExpressionSyntax ParsePrimaryExpression(SyntaxTokenStream stream)
    {
        var current = stream.Current;
        return current.SyntaxKind switch
        {
            >= SyntaxKind.AnyKeyword and <= SyntaxKind.F64Keyword => new SimpleNameSyntax(stream.Match()),
            >= SyntaxKind.TrueKeyword and <= SyntaxKind.NullKeyword => ParseLiteralExpression(stream),
            >= SyntaxKind.I8LiteralToken and <= SyntaxKind.StrLiteralToken => ParseLiteralExpression(stream),
            SyntaxKind.ParenthesisOpenToken when stream.IsLambdaExpressionAhead() => ParseLambdaExpression(stream),
            SyntaxKind.ParenthesisOpenToken => ParseGroupExpression(stream),
            //_ when iterator.Peek(1).SyntaxKind is SyntaxKind.ColonToken => ParseLocalDeclaration(iterator),
            _ => ParseName(stream),
        };
    }
}
