using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax, BinderContext context)
    {
        _ = context;
        var (kind, type) = syntax.SyntaxKind switch
        {
            SyntaxKind.I32LiteralExpression => (BoundKind.I32LiteralExpression, PredefinedTypes.I32),
            SyntaxKind.U32LiteralExpression => (BoundKind.U32LiteralExpression, PredefinedTypes.U32),
            SyntaxKind.I64LiteralExpression => (BoundKind.I64LiteralExpression, PredefinedTypes.I64),
            SyntaxKind.U64LiteralExpression => (BoundKind.U64LiteralExpression, PredefinedTypes.U64),
            SyntaxKind.F32LiteralExpression => (BoundKind.F32LiteralExpression, PredefinedTypes.F32),
            SyntaxKind.F64LiteralExpression => (BoundKind.F64LiteralExpression, PredefinedTypes.F64),
            SyntaxKind.StrLiteralExpression => (BoundKind.StrLiteralExpression, PredefinedTypes.Str),
            SyntaxKind.TrueLiteralExpression => (BoundKind.TrueLiteralExpression, PredefinedTypes.Bool),
            SyntaxKind.FalseLiteralExpression => (BoundKind.FalseLiteralExpression, PredefinedTypes.Bool),
            SyntaxKind.NullLiteralExpression => (BoundKind.NullLiteralExpression, PredefinedTypes.Unit),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        return new BoundLiteralExpression(kind, syntax, type, syntax.LiteralValue);
    }
}
