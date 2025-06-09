using System.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static TypeSymbol BindType(TypeSyntax syntax, BindingContext context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.ArrayType => BindArrayType((ArrayTypeSyntax)syntax, context),
            SyntaxKind.LambdaType => BindLambdaType((LambdaTypeSyntax)syntax, context),
            SyntaxKind.NamedType => BindNamedType((NamedTypeSyntax)syntax, context),
            SyntaxKind.MaybeType => BindMaybeType((MaybeTypeSyntax)syntax, context),
            SyntaxKind.PointerType => BindPointerType((PointerTypeSyntax)syntax, context),
            SyntaxKind.PredefinedType => BindPredefinedType((PredefinedTypeSyntax)syntax, context),
            SyntaxKind.UnionType => BindUnionType((UnionTypeSyntax)syntax, context),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'"),
        };
    }
}
