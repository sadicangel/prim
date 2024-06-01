using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static PrimType BindType(TypeSyntax syntax, BindingContext context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.PredefinedType => BindPredefinedType((PredefinedTypeSyntax)syntax, context),
            SyntaxKind.NamedType => BindNamedType((NamedTypeSyntax)syntax, context),
            SyntaxKind.OptionType => BindOptionType((OptionTypeSyntax)syntax, context),
            SyntaxKind.ArrayType => BindArrayType((ArrayTypeSyntax)syntax, context),
            SyntaxKind.FunctionType => BindFunctionType((FunctionTypeSyntax)syntax, context),
            SyntaxKind.UnionType => BindUnionType((UnionTypeSyntax)syntax, context),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'"),
        };

        static PredefinedType BindPredefinedType(PredefinedTypeSyntax syntax, BindingContext context)
        {
            _ = context;
            return syntax.PredefinedTypeToken.SyntaxKind switch
            {
                SyntaxKind.AnyKeyword => PredefinedTypes.Any,
                SyntaxKind.UnknownKeyword => PredefinedTypes.Unknown,
                SyntaxKind.NeverKeyword => PredefinedTypes.Never,
                SyntaxKind.UnitKeyword => PredefinedTypes.Unit,
                SyntaxKind.TypeKeyword => PredefinedTypes.Type,
                SyntaxKind.StrKeyword => PredefinedTypes.Str,
                SyntaxKind.BoolKeyword => PredefinedTypes.Bool,
                SyntaxKind.I8Keyword => PredefinedTypes.I8,
                SyntaxKind.I16Keyword => PredefinedTypes.I16,
                SyntaxKind.I32Keyword => PredefinedTypes.I32,
                SyntaxKind.I64Keyword => PredefinedTypes.I64,
                SyntaxKind.I128Keyword => PredefinedTypes.I128,
                SyntaxKind.ISizeKeyword => PredefinedTypes.ISize,
                SyntaxKind.U8Keyword => PredefinedTypes.U8,
                SyntaxKind.U16Keyword => PredefinedTypes.U16,
                SyntaxKind.U32Keyword => PredefinedTypes.U32,
                SyntaxKind.U64Keyword => PredefinedTypes.U64,
                SyntaxKind.U128Keyword => PredefinedTypes.U128,
                SyntaxKind.USizeKeyword => PredefinedTypes.USize,
                SyntaxKind.F16Keyword => PredefinedTypes.F16,
                SyntaxKind.F32Keyword => PredefinedTypes.F32,
                SyntaxKind.F64Keyword => PredefinedTypes.F64,
                SyntaxKind.F80Keyword => PredefinedTypes.F80,
                SyntaxKind.F128Keyword => PredefinedTypes.F128,
                _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'"),
            };
        }

        static PrimType BindNamedType(NamedTypeSyntax syntax, BindingContext context)
        {
            var structName = syntax.IdentifierToken.Text.ToString();
            if (context.BoundScope.Lookup(structName) is not StructSymbol structSymbol)
            {
                context.Diagnostics.ReportUndefinedType(syntax.IdentifierToken.Location, structName);
                return PredefinedTypes.Never;
            }
            return structSymbol.Type;
        }

        static PrimType BindOptionType(OptionTypeSyntax syntax, BindingContext context)
        {
            var underlyingType = BindType(syntax.UnderlyingType, context);
            if (underlyingType.IsNever)
                return underlyingType;
            return new OptionType(underlyingType);
        }

        static PrimType BindArrayType(ArrayTypeSyntax syntax, BindingContext context)
        {
            var elementType = BindType(syntax.ElementType, context);
            if (elementType.IsNever)
                return elementType;

            // TODO: Allow any constant value coercible to i32.
            if (syntax.SyntaxKind is not SyntaxKind.I32LiteralExpression)
            {
                context.Diagnostics.ReportInvalidArrayLength(syntax.Length.Location);
                return PredefinedTypes.Never;
            }

            var length = (BoundLiteralExpression)BindExpression(syntax.Length, context);
            Debug.Assert(length.Value is int);

            return new ArrayType(elementType, (int)length.Value!);
        }

        static PrimType BindFunctionType(FunctionTypeSyntax syntax, BindingContext context)
        {
            var returnType = BindType(syntax.ReturnType, context);
            if (!returnType.IsNever)
                return returnType;


            var parameters = new List<Parameter>(syntax.Parameters.Count);

            using (context.PushScope())
            {
                foreach (var parameterSyntax in syntax.Parameters)
                {
                    var parameter = BindParameter(parameterSyntax, context);
                    if (parameter.Type.IsNever)
                        return parameter.Type;
                    parameters.Add(parameter);
                }
            }

            return new FunctionType(new(parameters), returnType);

            static Parameter BindParameter(ParameterSyntax syntax, BindingContext context)
            {
                var parameterName = syntax.IdentifierToken.Text.ToString();
                var parameterType = BindType(syntax.Type, context);
                var parameterSymbol = new ParameterSymbol(syntax, parameterName, parameterType);
                if (!context.BoundScope.Declare(parameterSymbol))
                    context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, parameterName);
                return new Parameter(parameterName, parameterType);
            }
        }

        static PrimType BindUnionType(UnionTypeSyntax syntax, BindingContext context)
        {
            var types = new List<PrimType>(syntax.Types.Count);
            foreach (var typeSyntax in syntax.Types)
            {
                var type = BindType(typeSyntax, context);
                if (type.IsNever)
                    return type;
                types.Add(type);
            }
            return new UnionType(new(types));
        }
    }
}
