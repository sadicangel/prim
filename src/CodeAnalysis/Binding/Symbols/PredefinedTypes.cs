﻿using System.Reflection;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

// TODO: Merge this into global scope.
internal static class PredefinedTypes
{
    public static readonly StructTypeSymbol Any = new(SyntaxFactory.SyntheticToken(SyntaxKind.AnyKeyword), PredefinedTypeNames.Any);
    public static readonly StructTypeSymbol Unknown = new(SyntaxFactory.SyntheticToken(SyntaxKind.UnknownKeyword), PredefinedTypeNames.Unknown);
    public static readonly StructTypeSymbol Never = new(SyntaxFactory.SyntheticToken(SyntaxKind.NeverKeyword), PredefinedTypeNames.Never);
    public static readonly StructTypeSymbol Unit = new(SyntaxFactory.SyntheticToken(SyntaxKind.UnitKeyword), PredefinedTypeNames.Unit);
    public static readonly StructTypeSymbol Str = new(SyntaxFactory.SyntheticToken(SyntaxKind.StrKeyword), PredefinedTypeNames.Str);
    public static readonly StructTypeSymbol Bool = new(SyntaxFactory.SyntheticToken(SyntaxKind.BoolKeyword), PredefinedTypeNames.Bool);
    public static readonly StructTypeSymbol I8 = new(SyntaxFactory.SyntheticToken(SyntaxKind.I8Keyword), PredefinedTypeNames.I8);
    public static readonly StructTypeSymbol I16 = new(SyntaxFactory.SyntheticToken(SyntaxKind.I16Keyword), PredefinedTypeNames.I16);
    public static readonly StructTypeSymbol I32 = new(SyntaxFactory.SyntheticToken(SyntaxKind.I32Keyword), PredefinedTypeNames.I32);
    public static readonly StructTypeSymbol I64 = new(SyntaxFactory.SyntheticToken(SyntaxKind.I64Keyword), PredefinedTypeNames.I64);
    public static readonly StructTypeSymbol I128 = new(SyntaxFactory.SyntheticToken(SyntaxKind.I128Keyword), PredefinedTypeNames.I128);
    public static readonly StructTypeSymbol ISize = new(SyntaxFactory.SyntheticToken(SyntaxKind.ISizeKeyword), PredefinedTypeNames.ISize);
    public static readonly StructTypeSymbol U8 = new(SyntaxFactory.SyntheticToken(SyntaxKind.U8Keyword), PredefinedTypeNames.U8);
    public static readonly StructTypeSymbol U16 = new(SyntaxFactory.SyntheticToken(SyntaxKind.U16Keyword), PredefinedTypeNames.U16);
    public static readonly StructTypeSymbol U32 = new(SyntaxFactory.SyntheticToken(SyntaxKind.U32Keyword), PredefinedTypeNames.U32);
    public static readonly StructTypeSymbol U64 = new(SyntaxFactory.SyntheticToken(SyntaxKind.U64Keyword), PredefinedTypeNames.U64);
    public static readonly StructTypeSymbol U128 = new(SyntaxFactory.SyntheticToken(SyntaxKind.U128Keyword), PredefinedTypeNames.U128);
    public static readonly StructTypeSymbol USize = new(SyntaxFactory.SyntheticToken(SyntaxKind.USizeKeyword), PredefinedTypeNames.USize);
    public static readonly StructTypeSymbol F16 = new(SyntaxFactory.SyntheticToken(SyntaxKind.F16Keyword), PredefinedTypeNames.F16);
    public static readonly StructTypeSymbol F32 = new(SyntaxFactory.SyntheticToken(SyntaxKind.F32Keyword), PredefinedTypeNames.F32);
    public static readonly StructTypeSymbol F64 = new(SyntaxFactory.SyntheticToken(SyntaxKind.F64Keyword), PredefinedTypeNames.F64);
    public static readonly StructTypeSymbol F80 = new(SyntaxFactory.SyntheticToken(SyntaxKind.F80Keyword), PredefinedTypeNames.F80);
    public static readonly StructTypeSymbol F128 = new(SyntaxFactory.SyntheticToken(SyntaxKind.F128Keyword), PredefinedTypeNames.F128);
    public static readonly StructTypeSymbol Type = StructTypeSymbol.RuntimeType;

    static PredefinedTypes()
    {
        Str
            .AddEqualityOperators()
            .AddMembers(type =>
            {
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", Str), new Parameter("y", Str)], Str));
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", Str), new Parameter("y", Any)], Str));
                type.AddOperator(
                    SyntaxKind.PlusToken,
                    new LambdaTypeSymbol([new Parameter("x", Any), new Parameter("y", Str)], Str));
            });

        Bool
            .AddEqualityOperators()
            .AddLogicalOperators();

        I8
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(I16, I32, I64, I128, ISize, F16, F32, F64, F80, F128)
            .AddExplicitConversion(U8, U16, U32, U64, U128, USize);

        I16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(I32, I64, I128, ISize, F16, F32, F64, F80, F128)
            .AddExplicitConversion(I8, U8, U16, U32, U64, U128, USize);

        I32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(I64, I128, ISize, F32, F64, F80, F128)
            .AddExplicitConversion(I8, I16, U8, U16, U32, U64, U128, USize, F16);

        I64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(I128, ISize, F32, F64, F80, F128)
            .AddExplicitConversion(I8, I16, I32, U8, U16, U32, U64, U128, USize, F16);

        I128
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(I8, I16, I32, I64, ISize, U8, U16, U32, U64, U128, USize, F16, F32, F64, F80, F128);

        ISize
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(I8, I16, I32, I64, I128, U8, U16, U32, U64, U128, USize, F16, F32, F64, F80, F128);


        U8
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(U16, U32, U64, U128, USize, F16, F32, F64, F80, F128)
            .AddExplicitConversion(I8, I16, I32, I64, I128, ISize);

        U16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(U32, U64, U128, USize, F16, F32, F64, F80, F128)
            .AddExplicitConversion(U8, I8, I16, I32, I64, I128, ISize);

        U32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(U64, U128, USize, F32, F64, F80, F128)
            .AddExplicitConversion(U8, U16, I8, I16, I32, I64, I128, ISize, F16);

        U64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddImplicitConversion(U128, USize, F32, F64, F80, F128)
            .AddExplicitConversion(U8, U16, U32, I8, I16, I32, I64, I128, ISize, F16);

        U128
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, USize, F16, F32, F64, F80, F128);

        USize
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddBitwiseOperators()
            .AddMathOperators()
            .AddExplicitConversion(I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, F16, F32, F64, F80, F128);

        F16
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(F32, F64, F80, F128)
            .AddExplicitConversion(I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize);

        F32
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(F64, F80, F128)
            .AddExplicitConversion(F16, I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize);

        F64
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(F80, F128)
            .AddExplicitConversion(F16, F32, I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize);

        F80
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddImplicitConversion(F128)
            .AddExplicitConversion(F16, F32, F64, I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize);

        F128
            .AddEqualityOperators()
            .AddComparisonOperators()
            .AddMathOperators()
            .AddExplicitConversion(F16, F32, F64, F80, I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize);
    }

    public static IEnumerable<Symbol> All() => typeof(PredefinedTypes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => (Symbol)f.GetValue(null)!);

    private static TypeSymbol AddMembers(this TypeSymbol type, Action<TypeSymbol> add)
    {
        add(type);
        return type;
    }

    private static TypeSymbol AddMathOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.PlusToken,
            new LambdaTypeSymbol([new("x", type)], type));
        type.AddOperator(
            SyntaxKind.MinusToken,
            new LambdaTypeSymbol([new("x", type)], type));
        type.AddOperator(
            SyntaxKind.PlusToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.MinusToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.StarToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.SlashToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.PercentToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.StarStarToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        return type;
    }

    private static TypeSymbol AddBitwiseOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.TildeToken,
            new LambdaTypeSymbol([new("x", type)], type));
        type.AddOperator(
            SyntaxKind.AmpersandToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.PipeToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.HatToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.LessThanLessThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.GreaterThanGreaterThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], type));
        return type;
    }

    private static TypeSymbol AddEqualityOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.EqualsEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool));
        type.AddOperator(
            SyntaxKind.BangEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool));
        return type;
    }

    private static TypeSymbol AddComparisonOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.LessThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool));
        type.AddOperator(
            SyntaxKind.LessThanEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool));
        type.AddOperator(
            SyntaxKind.GreaterThanToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool));
        type.AddOperator(
            SyntaxKind.GreaterThanEqualsToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool));
        return type;
    }

    private static TypeSymbol AddLogicalOperators(this TypeSymbol type)
    {
        type.AddOperator(
            SyntaxKind.BangToken,
            new LambdaTypeSymbol([new("x", type)], Bool));
        type.AddOperator(
            SyntaxKind.AmpersandAmpersandToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool));
        type.AddOperator(
            SyntaxKind.PipePipeToken,
            new LambdaTypeSymbol([new("x", type), new("y", type)], Bool));
        return type;
    }

    private static TypeSymbol AddImplicitConversion(this TypeSymbol type, params ReadOnlySpan<TypeSymbol> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ImplicitKeyword, new LambdaTypeSymbol([new Parameter("x", type)], targetType));
        return type;
    }

    private static TypeSymbol AddExplicitConversion(this TypeSymbol type, params ReadOnlySpan<TypeSymbol> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ExplicitKeyword, new LambdaTypeSymbol([new Parameter("x", type)], targetType));
        return type;
    }
}

