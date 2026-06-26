using System.Collections.Immutable;
using System.Diagnostics;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal static class BinderTypeExtensions
{
    extension(Binder binder)
    {
        public TypeSymbol BindType(TypeSyntax syntax)
        {
            return syntax.Kind switch
            {
                SyntaxKind.ArrayType => binder.BindArrayType((ArrayTypeSyntax)syntax),
                SyntaxKind.LambdaType => binder.BindLambdaType((LambdaTypeSyntax)syntax),
                SyntaxKind.MaybeType => binder.BindMaybeType((MaybeTypeSyntax)syntax),
                SyntaxKind.NamedType => binder.BindNamedType((NamedTypeSyntax)syntax),
                SyntaxKind.PointerType => binder.BindPointerType((PointerTypeSyntax)syntax),
                >= SyntaxKind.AnyType and <= SyntaxKind.F64Type => binder.BindPredefinedType((PredefinedTypeSyntax)syntax),
                SyntaxKind.UnionType => binder.BindUnionType((UnionTypeSyntax)syntax),
                _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.Kind}'"),
            };
        }

        private ArrayTypeSymbol BindArrayType(ArrayTypeSyntax syntax)
        {
            var elementType = binder.BindType(syntax.ElementType);

            switch (syntax.Length)
            {
                // TODO: This should probably be isz, not i32.
                case LiteralExpressionSyntax { Kind: SyntaxKind.I32LiteralExpression } literal:
                    Debug.Assert(literal.InstanceValue is int);
                    return new ArrayTypeSymbol(syntax, elementType, (int)literal.InstanceValue, binder.Module);

                case null:
                    return new ArrayTypeSymbol(syntax, elementType, null, binder.Module);

                default:
                    binder.ReportInvalidArrayLength(syntax.Length.SourceSpan);
                    return new ArrayTypeSymbol(syntax, binder.Module.Never, Length: 0, binder.Module);
            }
        }

        private LambdaTypeSymbol BindLambdaType(LambdaTypeSyntax syntax)
        {
            var parameters = syntax.Parameters.Select(binder.BindType).ToImmutableArray();
            var returnType = binder.BindType(syntax.ReturnType);

            return new LambdaTypeSymbol(syntax, parameters, returnType, binder.Module);
        }

        private UnionTypeSymbol BindMaybeType(MaybeTypeSyntax syntax)
        {
            var underlyingType = binder.BindType(syntax.UnderlyingType);

            return new UnionTypeSymbol(syntax, [binder.Module.Unit, underlyingType], binder.Module);
        }

        private StructTypeSymbol BindNamedType(NamedTypeSyntax syntax)
        {
            if (binder.TryLookupNamedType(syntax.Name, out var structType))
            {
                return structType;
            }

            binder.ReportUndefinedType(syntax.Name.SourceSpan, syntax.Name.FullName);

            return binder.Module.Never;
        }

        private bool TryLookupNamedType(NameSyntax syntax, out StructTypeSymbol structType)
        {
            var parts = syntax.Name.ToArray();
            if (parts.Length == 1)
                return binder.Module.TryLookup(parts[0], out structType);

            if (!binder.TryLookup<ModuleSymbol>(parts[0], out var module))
            {
                structType = null!;
                return false;
            }

            for (var i = 1; i < parts.Length - 1; i++)
            {
                if (!module.TryLookup<ModuleSymbol>(parts[i], out var childModule))
                {
                    structType = null!;
                    return false;
                }

                module = childModule;
            }

            return module.TryLookup(parts[^1], out structType);
        }
        private PointerTypeSymbol BindPointerType(PointerTypeSyntax syntax)
        {
            var elementType = binder.BindType(syntax.ElementType);
            return new PointerTypeSymbol(syntax, elementType, binder.Module);
        }

        private StructTypeSymbol BindPredefinedType(PredefinedTypeSyntax syntax)
        {
            return syntax.PredefinedTypeToken.Kind switch
            {
                SyntaxKind.AnyKeyword => binder.Module.Any,
                SyntaxKind.UnknownKeyword => binder.Module.Unknown,
                SyntaxKind.NeverKeyword => binder.Module.Never,
                SyntaxKind.UnitKeyword => binder.Module.Unit,
                SyntaxKind.TypeKeyword => binder.Module.RuntimeType,
                SyntaxKind.StrKeyword => binder.Module.Str,
                SyntaxKind.BoolKeyword => binder.Module.Bool,
                SyntaxKind.I8Keyword => binder.Module.I8,
                SyntaxKind.I16Keyword => binder.Module.I16,
                SyntaxKind.I32Keyword => binder.Module.I32,
                SyntaxKind.I64Keyword => binder.Module.I64,
                SyntaxKind.IszKeyword => binder.Module.Isz,
                SyntaxKind.U8Keyword => binder.Module.U8,
                SyntaxKind.U16Keyword => binder.Module.U16,
                SyntaxKind.U32Keyword => binder.Module.U32,
                SyntaxKind.U64Keyword => binder.Module.U64,
                SyntaxKind.UszKeyword => binder.Module.Usz,
                SyntaxKind.F16Keyword => binder.Module.F16,
                SyntaxKind.F32Keyword => binder.Module.F32,
                SyntaxKind.F64Keyword => binder.Module.F64,
                _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.Kind}'"),
            };
        }

        private UnionTypeSymbol BindUnionType(UnionTypeSyntax syntax)
        {
            var types = syntax.Types.Select(binder.BindType).ToImmutableArray();

            return new UnionTypeSymbol(syntax, types, binder.Module);
        }
    }
}
