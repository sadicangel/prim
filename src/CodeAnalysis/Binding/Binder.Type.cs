using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static TypeSymbol BindType(TypeSyntax syntax, BinderContext context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.PredefinedType => BindPredefinedType((PredefinedTypeSyntax)syntax, context),
            SyntaxKind.NamedType => BindNamedType((NamedTypeSyntax)syntax, context),
            SyntaxKind.OptionType => BindOptionType((OptionTypeSyntax)syntax, context),
            SyntaxKind.ArrayType => BindArrayType((ArrayTypeSyntax)syntax, context),
            SyntaxKind.LambdaType => BindLambdaType((LambdaTypeSyntax)syntax, context),
            SyntaxKind.UnionType => BindUnionType((UnionTypeSyntax)syntax, context),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'"),
        };
    }

    static TypeSymbol BindPredefinedType(PredefinedTypeSyntax syntax, BinderContext context)
    {
        _ = context;
        return syntax.PredefinedTypeToken.SyntaxKind switch
        {
            SyntaxKind.AnyKeyword => PredefinedSymbols.Any,
            SyntaxKind.UnknownKeyword => PredefinedSymbols.Unknown,
            SyntaxKind.NeverKeyword => PredefinedSymbols.Never,
            SyntaxKind.UnitKeyword => PredefinedSymbols.Unit,
            SyntaxKind.TypeKeyword => PredefinedSymbols.Type,
            SyntaxKind.StrKeyword => PredefinedSymbols.Str,
            SyntaxKind.BoolKeyword => PredefinedSymbols.Bool,
            SyntaxKind.I8Keyword => PredefinedSymbols.I8,
            SyntaxKind.I16Keyword => PredefinedSymbols.I16,
            SyntaxKind.I32Keyword => PredefinedSymbols.I32,
            SyntaxKind.I64Keyword => PredefinedSymbols.I64,
            SyntaxKind.I128Keyword => PredefinedSymbols.I128,
            SyntaxKind.ISizeKeyword => PredefinedSymbols.ISize,
            SyntaxKind.U8Keyword => PredefinedSymbols.U8,
            SyntaxKind.U16Keyword => PredefinedSymbols.U16,
            SyntaxKind.U32Keyword => PredefinedSymbols.U32,
            SyntaxKind.U64Keyword => PredefinedSymbols.U64,
            SyntaxKind.U128Keyword => PredefinedSymbols.U128,
            SyntaxKind.USizeKeyword => PredefinedSymbols.USize,
            SyntaxKind.F16Keyword => PredefinedSymbols.F16,
            SyntaxKind.F32Keyword => PredefinedSymbols.F32,
            SyntaxKind.F64Keyword => PredefinedSymbols.F64,
            SyntaxKind.F80Keyword => PredefinedSymbols.F80,
            SyntaxKind.F128Keyword => PredefinedSymbols.F128,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'"),
        };
    }

    static StructTypeSymbol BindNamedType(NamedTypeSyntax syntax, BinderContext context)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(structName) is not StructTypeSymbol typeSymbol)
        {
            context.Diagnostics.ReportUndefinedType(syntax.IdentifierToken.Location, structName);
            return PredefinedSymbols.Never;
        }
        return typeSymbol;
    }

    static OptionTypeSymbol BindOptionType(OptionTypeSyntax syntax, BinderContext context)
    {
        var underlyingType = BindType(syntax.UnderlyingType, context);
        return new OptionTypeSymbol(syntax, underlyingType);
    }

    static ArrayTypeSymbol BindArrayType(ArrayTypeSyntax syntax, BinderContext context)
    {
        var elementType = BindType(syntax.ElementType, context);

        // TODO: Allow any constant value coercible to i32 (or isize?).
        if (syntax.Length.SyntaxKind is SyntaxKind.I32LiteralExpression)
        {
            var length = (BoundLiteralExpression)BindExpression(syntax.Length, context);
            Debug.Assert(length.Value is int);
            return new ArrayTypeSymbol(syntax, elementType, (int)length.Value!);
        }

        context.Diagnostics.ReportInvalidArrayLength(syntax.Length.Location);
        return new ArrayTypeSymbol(syntax, PredefinedSymbols.Never, length: 0);
    }

    static LambdaTypeSymbol BindLambdaType(LambdaTypeSyntax syntax, BinderContext context)
    {
        var returnType = BindType(syntax.ReturnType, context);

        var parameters = new List<Parameter>(syntax.Parameters.Count);

        var seenParameterNames = new HashSet<string>();
        foreach (var parameterSyntax in syntax.Parameters)
        {
            var parameterName = parameterSyntax.IdentifierToken.Text.ToString();
            var parameterType = BindType(parameterSyntax.Type, context);
            if (!seenParameterNames.Add(parameterName))
                context.Diagnostics.ReportSymbolRedeclaration(syntax.Location, parameterName);
            var parameter = new Parameter(parameterSyntax, parameterName, parameterType);
            parameters.Add(parameter);
        }

        return new LambdaTypeSymbol(syntax, parameters, returnType);
    }

    static UnionTypeSymbol BindUnionType(UnionTypeSyntax syntax, BinderContext context)
    {
        var types = new BoundList<TypeSymbol>.Builder(syntax.Types.Count);
        foreach (var typeSyntax in syntax.Types)
            types.Add(BindType(typeSyntax, context));
        return new UnionTypeSymbol(syntax, types.ToBoundList());
    }
}
