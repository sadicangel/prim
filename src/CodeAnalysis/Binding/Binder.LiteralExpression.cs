using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
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
            SyntaxKind.I8LiteralExpression => Predefined.I8,
            SyntaxKind.I16LiteralExpression => Predefined.I16,
            SyntaxKind.I32LiteralExpression => Predefined.I32,
            SyntaxKind.I64LiteralExpression => Predefined.I64,
            SyntaxKind.U8LiteralExpression => Predefined.U8,
            SyntaxKind.U16LiteralExpression => Predefined.U16,
            SyntaxKind.U32LiteralExpression => Predefined.U32,
            SyntaxKind.U64LiteralExpression => Predefined.U64,
            SyntaxKind.F16LiteralExpression => Predefined.F16,
            SyntaxKind.F32LiteralExpression => Predefined.F32,
            SyntaxKind.F64LiteralExpression => Predefined.F64,
            SyntaxKind.StrLiteralExpression => Predefined.Str,
            SyntaxKind.TrueLiteralExpression => Predefined.Bool,
            SyntaxKind.FalseLiteralExpression => Predefined.Bool,
            SyntaxKind.NullLiteralExpression => Predefined.Unit,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        return new BoundLiteralExpression(syntax, type, syntax.InstanceValue);
    }
}
