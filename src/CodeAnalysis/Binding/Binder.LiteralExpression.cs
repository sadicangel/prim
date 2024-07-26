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
            SyntaxKind.I32LiteralExpression => PredefinedSymbols.I32,
            SyntaxKind.U32LiteralExpression => PredefinedSymbols.U32,
            SyntaxKind.I64LiteralExpression => PredefinedSymbols.I64,
            SyntaxKind.U64LiteralExpression => PredefinedSymbols.U64,
            SyntaxKind.F32LiteralExpression => PredefinedSymbols.F32,
            SyntaxKind.F64LiteralExpression => PredefinedSymbols.F64,
            SyntaxKind.StrLiteralExpression => PredefinedSymbols.Str,
            SyntaxKind.TrueLiteralExpression => PredefinedSymbols.Bool,
            SyntaxKind.FalseLiteralExpression => PredefinedSymbols.Bool,
            SyntaxKind.NullLiteralExpression => PredefinedSymbols.Unit,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'")
        };

        return new BoundLiteralExpression(syntax, type, syntax.LiteralValue);
    }
}
