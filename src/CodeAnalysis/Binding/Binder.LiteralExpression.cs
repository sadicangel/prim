using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax, BinderContext context)
    {
        _ = context;
        var type = syntax.SyntaxKind switch
        {
            SyntaxKind.I32LiteralExpression => PredefinedTypes.I32,
            SyntaxKind.U32LiteralExpression => PredefinedTypes.U32,
            SyntaxKind.I64LiteralExpression => PredefinedTypes.I64,
            SyntaxKind.U64LiteralExpression => PredefinedTypes.U64,
            SyntaxKind.F32LiteralExpression => PredefinedTypes.F32,
            SyntaxKind.F64LiteralExpression => PredefinedTypes.F64,
            SyntaxKind.StrLiteralExpression => PredefinedTypes.Str,
            SyntaxKind.TrueLiteralExpression => PredefinedTypes.Bool,
            SyntaxKind.FalseLiteralExpression => PredefinedTypes.Bool,
            SyntaxKind.NullLiteralExpression => PredefinedTypes.Unit,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        return new BoundLiteralExpression(syntax, type, syntax.LiteralValue);
    }
}
