using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax, Context context)
    {
        _ = context;
        var type = syntax.SyntaxKind switch
        {
            SyntaxKind.I8LiteralExpression => context.BoundScope.I8,
            SyntaxKind.I16LiteralExpression => context.BoundScope.I16,
            SyntaxKind.I32LiteralExpression => context.BoundScope.I32,
            SyntaxKind.I64LiteralExpression => context.BoundScope.I64,
            SyntaxKind.U8LiteralExpression => context.BoundScope.U8,
            SyntaxKind.U16LiteralExpression => context.BoundScope.U16,
            SyntaxKind.U32LiteralExpression => context.BoundScope.U32,
            SyntaxKind.U64LiteralExpression => context.BoundScope.U64,
            SyntaxKind.F16LiteralExpression => context.BoundScope.F16,
            SyntaxKind.F32LiteralExpression => context.BoundScope.F32,
            SyntaxKind.F64LiteralExpression => context.BoundScope.F64,
            SyntaxKind.StrLiteralExpression => context.BoundScope.Str,
            SyntaxKind.TrueLiteralExpression => context.BoundScope.Bool,
            SyntaxKind.FalseLiteralExpression => context.BoundScope.Bool,
            SyntaxKind.NullLiteralExpression => context.BoundScope.Unit,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        return new BoundLiteralExpression(syntax, type, syntax.InstanceValue);
    }
}
