using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;
using System.Diagnostics;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static LiteralExpressionSyntax ParseLiteralExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var literalToken = iterator.Next();
        return literalToken.SyntaxKind switch
        {
            SyntaxKind.I32LiteralToken => new(SyntaxKind.I32LiteralExpression, syntaxTree, literalToken, PredefinedTypes.I32, literalToken.Value),
            SyntaxKind.I64LiteralToken => new(SyntaxKind.I64LiteralExpression, syntaxTree, literalToken, PredefinedTypes.I64, literalToken.Value),
            SyntaxKind.F32LiteralToken => new(SyntaxKind.F32LiteralExpression, syntaxTree, literalToken, PredefinedTypes.F32, literalToken.Value),
            SyntaxKind.F64LiteralToken => new(SyntaxKind.F64LiteralExpression, syntaxTree, literalToken, PredefinedTypes.F64, literalToken.Value),
            SyntaxKind.StrLiteralToken => new(SyntaxKind.StrLiteralExpression, syntaxTree, literalToken, PredefinedTypes.Str, literalToken.Value),
            SyntaxKind.TrueLiteralToken => new(SyntaxKind.TrueLiteralExpression, syntaxTree, literalToken, PredefinedTypes.Bool, true),
            SyntaxKind.FalseLiteralToken => new(SyntaxKind.FalseLiteralExpression, syntaxTree, literalToken, PredefinedTypes.Bool, false),
            SyntaxKind.NullLiteralToken => new(SyntaxKind.NullLiteralExpression, syntaxTree, literalToken, PredefinedTypes.Unit, null),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{literalToken.SyntaxKind}' for literal")
        };
    }
}
