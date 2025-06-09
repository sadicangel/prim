using System.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static StructSymbol BindPredefinedType(PredefinedTypeSyntax syntax, BindingContext context)
    {
        _ = context;
        return syntax.PredefinedTypeToken.SyntaxKind switch
        {
            SyntaxKind.AnyKeyword => context.Module.Any,
            SyntaxKind.UnknownKeyword => context.Module.Unknown,
            SyntaxKind.NeverKeyword => context.Module.Never,
            SyntaxKind.UnitKeyword => context.Module.Unit,
            SyntaxKind.TypeKeyword => context.Module.RuntimeType,
            SyntaxKind.StrKeyword => context.Module.Str,
            SyntaxKind.BoolKeyword => context.Module.Bool,
            SyntaxKind.I8Keyword => context.Module.I8,
            SyntaxKind.I16Keyword => context.Module.I16,
            SyntaxKind.I32Keyword => context.Module.I32,
            SyntaxKind.I64Keyword => context.Module.I64,
            SyntaxKind.IszKeyword => context.Module.Isz,
            SyntaxKind.U8Keyword => context.Module.U8,
            SyntaxKind.U16Keyword => context.Module.U16,
            SyntaxKind.U32Keyword => context.Module.U32,
            SyntaxKind.U64Keyword => context.Module.U64,
            SyntaxKind.UszKeyword => context.Module.Usz,
            SyntaxKind.F16Keyword => context.Module.F16,
            SyntaxKind.F32Keyword => context.Module.F32,
            SyntaxKind.F64Keyword => context.Module.F64,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'"),
        };
    }
}
