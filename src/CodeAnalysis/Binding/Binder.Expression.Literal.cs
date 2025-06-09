using System.Diagnostics;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax, BindingContext context)
    {
        var type = syntax.SyntaxKind switch
        {
            SyntaxKind.I8LiteralExpression => context.Module.I8,
            SyntaxKind.I16LiteralExpression => context.Module.I16,
            SyntaxKind.I32LiteralExpression => context.Module.I32,
            SyntaxKind.I64LiteralExpression => context.Module.I64,
            SyntaxKind.U8LiteralExpression => context.Module.U8,
            SyntaxKind.U16LiteralExpression => context.Module.U16,
            SyntaxKind.U32LiteralExpression => context.Module.U32,
            SyntaxKind.U64LiteralExpression => context.Module.U64,
            SyntaxKind.F16LiteralExpression => context.Module.F16,
            SyntaxKind.F32LiteralExpression => context.Module.F32,
            SyntaxKind.F64LiteralExpression => context.Module.F64,
            SyntaxKind.StrLiteralExpression => context.Module.Str,
            SyntaxKind.TrueLiteralExpression => context.Module.Bool,
            SyntaxKind.FalseLiteralExpression => context.Module.Bool,
            SyntaxKind.NullLiteralExpression => context.Module.Unit,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        return new BoundLiteralExpression(syntax, type, syntax.InstanceValue);
    }
}
