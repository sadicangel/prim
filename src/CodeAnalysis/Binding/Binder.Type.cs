using System.Collections.Immutable;
using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static TypeSymbol BindType(TypeSyntax syntax, Context context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.ArrayType => BindArrayType((ArrayTypeSyntax)syntax, context),
            SyntaxKind.ErrorType => BindErrorType((ErrorTypeSyntax)syntax, context),
            SyntaxKind.LambdaType => BindLambdaType((LambdaTypeSyntax)syntax, context),
            SyntaxKind.NamedType => BindNamedType((NamedTypeSyntax)syntax, context),
            SyntaxKind.OptionType => BindOptionType((OptionTypeSyntax)syntax, context),
            SyntaxKind.PointerType => BindPointerType((PointerTypeSyntax)syntax, context),
            SyntaxKind.PredefinedType => BindPredefinedType((PredefinedTypeSyntax)syntax, context),
            SyntaxKind.UnionType => BindUnionType((UnionTypeSyntax)syntax, context),
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'"),
        };
    }

    static ArrayTypeSymbol BindArrayType(ArrayTypeSyntax syntax, Context context)
    {
        var elementType = BindType(syntax.ElementType, context);

        // TODO: Allow any constant value coercible to i32 (or isz?).
        if (syntax.Length.SyntaxKind is SyntaxKind.I32LiteralExpression)
        {
            var length = (BoundLiteralExpression)BindExpression(syntax.Length, context);
            Debug.Assert(length.Value is int);
            return context.BoundScope.CreateArrayType(elementType, (int)length.Value!, syntax);
        }

        context.Diagnostics.ReportInvalidArrayLength(syntax.Length.Location);
        return context.BoundScope.CreateArrayType(context.BoundScope.Never, length: 0, syntax);
    }

    static ErrorTypeSymbol BindErrorType(ErrorTypeSyntax syntax, Context context)
    {
        var valueType = BindType(syntax.ValueType, context);
        return context.BoundScope.CreateErrorType(valueType, syntax);
    }

    static LambdaTypeSymbol BindLambdaType(LambdaTypeSyntax syntax, Context context)
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

        return context.BoundScope.CreateLambdaType(parameters, returnType, syntax);
    }

    static StructTypeSymbol BindNamedType(NamedTypeSyntax syntax, Context context)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(structName) is not StructTypeSymbol typeSymbol)
        {
            context.Diagnostics.ReportUndefinedType(syntax.IdentifierToken.Location, structName);
            return context.BoundScope.Never;
        }
        return typeSymbol;
    }

    static OptionTypeSymbol BindOptionType(OptionTypeSyntax syntax, Context context)
    {
        var underlyingType = BindType(syntax.UnderlyingType, context);
        return context.BoundScope.CreateOptionType(underlyingType, syntax);
    }

    static PointerTypeSymbol BindPointerType(PointerTypeSyntax syntax, Context context)
    {
        var elementType = BindType(syntax.ElementType, context);
        return context.BoundScope.CreatePointerType(elementType, syntax);
    }

    static StructTypeSymbol BindPredefinedType(PredefinedTypeSyntax syntax, Context context)
    {
        _ = context;
        return syntax.PredefinedTypeToken.SyntaxKind switch
        {
            SyntaxKind.AnyKeyword => context.BoundScope.Any,
            SyntaxKind.ErrKeyword => context.BoundScope.Err,
            SyntaxKind.UnknownKeyword => context.BoundScope.Unknown,
            SyntaxKind.NeverKeyword => context.BoundScope.Never,
            SyntaxKind.UnitKeyword => context.BoundScope.Unit,
            SyntaxKind.TypeKeyword => context.BoundScope.RuntimeType,
            SyntaxKind.StrKeyword => context.BoundScope.Str,
            SyntaxKind.BoolKeyword => context.BoundScope.Bool,
            SyntaxKind.I8Keyword => context.BoundScope.I8,
            SyntaxKind.I16Keyword => context.BoundScope.I16,
            SyntaxKind.I32Keyword => context.BoundScope.I32,
            SyntaxKind.I64Keyword => context.BoundScope.I64,
            SyntaxKind.I128Keyword => context.BoundScope.I128,
            SyntaxKind.IszKeyword => context.BoundScope.Isz,
            SyntaxKind.U8Keyword => context.BoundScope.U8,
            SyntaxKind.U16Keyword => context.BoundScope.U16,
            SyntaxKind.U32Keyword => context.BoundScope.U32,
            SyntaxKind.U64Keyword => context.BoundScope.U64,
            SyntaxKind.U128Keyword => context.BoundScope.U128,
            SyntaxKind.UszKeyword => context.BoundScope.Usz,
            SyntaxKind.F16Keyword => context.BoundScope.F16,
            SyntaxKind.F32Keyword => context.BoundScope.F32,
            SyntaxKind.F64Keyword => context.BoundScope.F64,
            SyntaxKind.F80Keyword => context.BoundScope.F80,
            SyntaxKind.F128Keyword => context.BoundScope.F128,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'"),
        };
    }

    static UnionTypeSymbol BindUnionType(UnionTypeSyntax syntax, Context context)
    {
        var builder = ImmutableArray.CreateBuilder<TypeSymbol>(syntax.Types.Count);
        foreach (var typeSyntax in syntax.Types)
            builder.Add(BindType(typeSyntax, context));
        var types = new BoundList<TypeSymbol>(builder.ToImmutable());
        return context.BoundScope.CreateUnionType(types, syntax);
    }
}
