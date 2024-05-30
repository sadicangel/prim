using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax, BindingContext context)
    {
        _ = context;
        var boundKind = syntax.SyntaxKind switch
        {
            SyntaxKind.I32LiteralExpression => BoundKind.I32LiteralExpression,
            SyntaxKind.U32LiteralExpression => BoundKind.U32LiteralExpression,
            SyntaxKind.I64LiteralExpression => BoundKind.I64LiteralExpression,
            SyntaxKind.U64LiteralExpression => BoundKind.U64LiteralExpression,
            SyntaxKind.F32LiteralExpression => BoundKind.F32LiteralExpression,
            SyntaxKind.F64LiteralExpression => BoundKind.F64LiteralExpression,
            SyntaxKind.StrLiteralExpression => BoundKind.StrLiteralExpression,
            SyntaxKind.TrueLiteralExpression => BoundKind.TrueLiteralExpression,
            SyntaxKind.FalseLiteralExpression => BoundKind.FalseLiteralExpression,
            SyntaxKind.NullLiteralExpression => BoundKind.NullLiteralExpression,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };
        return new BoundLiteralExpression(boundKind, syntax, syntax.LiteralValue);
    }
}
