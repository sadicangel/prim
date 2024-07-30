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

        // TODO: Allow any constant value coercible to i32 (or isize?).
        if (syntax.Length.SyntaxKind is SyntaxKind.I32LiteralExpression)
        {
            var length = (BoundLiteralExpression)BindExpression(syntax.Length, context);
            Debug.Assert(length.Value is int);
            return new ArrayTypeSymbol(syntax, elementType, (int)length.Value!, context.Module);
        }

        context.Diagnostics.ReportInvalidArrayLength(syntax.Length.Location);
        return new ArrayTypeSymbol(syntax, Predefined.Never, length: 0, context.Module);
    }

    static ErrorTypeSymbol BindErrorType(ErrorTypeSyntax syntax, Context context)
    {
        var valueType = BindType(syntax.ValueType, context);
        return new ErrorTypeSymbol(syntax, valueType, context.Module);
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

        return new LambdaTypeSymbol(syntax, parameters, returnType, context.Module);
    }

    static StructTypeSymbol BindNamedType(NamedTypeSyntax syntax, Context context)
    {
        var structName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(structName) is not StructTypeSymbol typeSymbol)
        {
            context.Diagnostics.ReportUndefinedType(syntax.IdentifierToken.Location, structName);
            return Predefined.Never;
        }
        return typeSymbol;
    }

    static OptionTypeSymbol BindOptionType(OptionTypeSyntax syntax, Context context)
    {
        var underlyingType = BindType(syntax.UnderlyingType, context);
        return new OptionTypeSymbol(syntax, underlyingType, context.Module);
    }

    static PointerTypeSymbol BindPointerType(PointerTypeSyntax syntax, Context context)
    {
        var elementType = BindType(syntax.ElementType, context);
        return new PointerTypeSymbol(syntax, elementType, context.Module);
    }

    static StructTypeSymbol BindPredefinedType(PredefinedTypeSyntax syntax, Context context)
    {
        _ = context;
        return syntax.PredefinedTypeToken.SyntaxKind switch
        {
            SyntaxKind.AnyKeyword => Predefined.Any,
            SyntaxKind.ErrKeyword => Predefined.Err,
            SyntaxKind.UnknownKeyword => Predefined.Unknown,
            SyntaxKind.NeverKeyword => Predefined.Never,
            SyntaxKind.UnitKeyword => Predefined.Unit,
            SyntaxKind.TypeKeyword => Predefined.Type,
            SyntaxKind.StrKeyword => Predefined.Str,
            SyntaxKind.BoolKeyword => Predefined.Bool,
            SyntaxKind.I8Keyword => Predefined.I8,
            SyntaxKind.I16Keyword => Predefined.I16,
            SyntaxKind.I32Keyword => Predefined.I32,
            SyntaxKind.I64Keyword => Predefined.I64,
            SyntaxKind.I128Keyword => Predefined.I128,
            SyntaxKind.ISizeKeyword => Predefined.ISize,
            SyntaxKind.U8Keyword => Predefined.U8,
            SyntaxKind.U16Keyword => Predefined.U16,
            SyntaxKind.U32Keyword => Predefined.U32,
            SyntaxKind.U64Keyword => Predefined.U64,
            SyntaxKind.U128Keyword => Predefined.U128,
            SyntaxKind.USizeKeyword => Predefined.USize,
            SyntaxKind.F16Keyword => Predefined.F16,
            SyntaxKind.F32Keyword => Predefined.F32,
            SyntaxKind.F64Keyword => Predefined.F64,
            SyntaxKind.F80Keyword => Predefined.F80,
            SyntaxKind.F128Keyword => Predefined.F128,
            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{syntax.SyntaxKind}'"),
        };
    }

    static UnionTypeSymbol BindUnionType(UnionTypeSyntax syntax, Context context)
    {
        var builder = ImmutableArray.CreateBuilder<TypeSymbol>(syntax.Types.Count);
        foreach (var typeSyntax in syntax.Types)
            builder.Add(BindType(typeSyntax, context));
        var types = new BoundList<TypeSymbol>(builder.ToImmutable());
        return new UnionTypeSymbol(syntax, types, context.Module);
    }
}
