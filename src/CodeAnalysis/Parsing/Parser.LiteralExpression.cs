using System.Diagnostics;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    public static LiteralExpressionSyntax ParseLiteralExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var literalToken = iterator.Next();

        return literalToken.SyntaxKind switch
        {
            SyntaxKind.TrueKeyword =>
                new LiteralExpressionSyntax(SyntaxKind.TrueLiteralExpression, syntaxTree, literalToken, PredefinedTypes.Bool, true),
            SyntaxKind.FalseKeyword =>
                new LiteralExpressionSyntax(SyntaxKind.FalseLiteralExpression, syntaxTree, literalToken, PredefinedTypes.Bool, false),
            SyntaxKind.NullKeyword =>
                new LiteralExpressionSyntax(SyntaxKind.NullLiteralExpression, syntaxTree, literalToken, PredefinedTypes.Unit, null),
            SyntaxKind.I32LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.I32LiteralExpression, syntaxTree, literalToken, PredefinedTypes.I32, literalToken.Value),
            SyntaxKind.U32LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.U32LiteralExpression, syntaxTree, literalToken, PredefinedTypes.U32, literalToken.Value),
            SyntaxKind.I64LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.I64LiteralExpression, syntaxTree, literalToken, PredefinedTypes.I64, literalToken.Value),
            SyntaxKind.U64LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.U64LiteralExpression, syntaxTree, literalToken, PredefinedTypes.U64, literalToken.Value),
            SyntaxKind.F32LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.F32LiteralExpression, syntaxTree, literalToken, PredefinedTypes.F32, literalToken.Value),
            SyntaxKind.F64LiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.F64LiteralExpression, syntaxTree, literalToken, PredefinedTypes.F64, literalToken.Value),
            SyntaxKind.StrLiteralToken =>
                new LiteralExpressionSyntax(SyntaxKind.StrLiteralExpression, syntaxTree, literalToken, PredefinedTypes.Str, literalToken.Value),
            _ =>
                throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{literalToken.SyntaxKind}' for {nameof(LiteralExpressionSyntax)}")
        };
    }
}
