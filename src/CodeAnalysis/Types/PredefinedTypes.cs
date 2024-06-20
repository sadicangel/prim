using CodeAnalysis.Syntax;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Types;
internal static class PredefinedTypes
{
    public static readonly StructType Any = new(PredefinedTypeNames.Any);
    public static readonly StructType Unknown = new(PredefinedTypeNames.Unknown);
    public static readonly StructType Never = new(PredefinedTypeNames.Never);
    public static readonly StructType Unit = new(PredefinedTypeNames.Unit);
    public static readonly StructType Type = new(PredefinedTypeNames.Type);
    public static readonly StructType Str = new(PredefinedTypeNames.Str);
    public static readonly StructType Bool = new(PredefinedTypeNames.Bool);
    public static readonly StructType I8 = new(PredefinedTypeNames.I8);
    public static readonly StructType I16 = new(PredefinedTypeNames.I16);
    public static readonly StructType I32 = new(PredefinedTypeNames.I32);
    public static readonly StructType I64 = new(PredefinedTypeNames.I64);
    public static readonly StructType I128 = new(PredefinedTypeNames.I128);
    public static readonly StructType ISize = new(PredefinedTypeNames.ISize);
    public static readonly StructType U8 = new(PredefinedTypeNames.U8);
    public static readonly StructType U16 = new(PredefinedTypeNames.U16);
    public static readonly StructType U32 = new(PredefinedTypeNames.U32);
    public static readonly StructType U64 = new(PredefinedTypeNames.U64);
    public static readonly StructType U128 = new(PredefinedTypeNames.U128);
    public static readonly StructType USize = new(PredefinedTypeNames.USize);
    public static readonly StructType F16 = new(PredefinedTypeNames.F16);
    public static readonly StructType F32 = new(PredefinedTypeNames.F32);
    public static readonly StructType F64 = new(PredefinedTypeNames.F64);
    public static readonly StructType F80 = new(PredefinedTypeNames.F80);
    public static readonly StructType F128 = new(PredefinedTypeNames.F128);

    static PredefinedTypes()
    {
        Str
            .AddEqualityOperators()
            .AddMembers(type =>
            {
                type.AddOperator(
                    SyntaxKind.AddOperator,
                    new FunctionType([new Parameter("x", Str), new Parameter("y", Str)], Str));
                type.AddOperator(
                    SyntaxKind.AddOperator,
                    new FunctionType([new Parameter("x", Str), new Parameter("y", Any)], Str));
                type.AddOperator(
                    SyntaxKind.AddOperator,
                    new FunctionType([new Parameter("x", Any), new Parameter("y", Str)], Str));
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
            .AddExplicitConversion(I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize, F16, F32, F64, F80, F128);

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

    private static PrimType AddMembers(this PrimType type, Action<PrimType> add)
    {
        add(type);
        return type;
    }

    private static PrimType AddMathOperators(this PrimType type)
    {
        type.AddOperator(
            SyntaxKind.UnaryPlusOperator,
            new FunctionType([new("x", type)], type));
        type.AddOperator(
            SyntaxKind.UnaryMinusOperator,
            new FunctionType([new("x", type)], type));
        type.AddOperator(
            SyntaxKind.AddOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.SubtractOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.MultiplyOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.DivideOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.ModuloOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.PowerOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        return type;
    }

    private static PrimType AddBitwiseOperators(this PrimType type)
    {
        type.AddOperator(
            SyntaxKind.OnesComplementOperator,
            new FunctionType([new("x", type)], type));
        type.AddOperator(
            SyntaxKind.BitwiseAndOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.BitwiseOrOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.ExclusiveOrOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.LeftShiftOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        type.AddOperator(
            SyntaxKind.RightShiftOperator,
            new FunctionType([new("x", type), new("y", type)], type));
        return type;
    }

    private static PrimType AddEqualityOperators(this PrimType type)
    {
        type.AddOperator(
            SyntaxKind.EqualsOperator,
            new FunctionType([new("x", type), new("y", type)], Bool));
        type.AddOperator(
            SyntaxKind.NotEqualsOperator,
            new FunctionType([new("x", type), new("y", type)], Bool));
        return type;
    }

    private static PrimType AddComparisonOperators(this PrimType type)
    {
        type.AddOperator(
            SyntaxKind.LessThanOperator,
            new FunctionType([new("x", type), new("y", type)], Bool));
        type.AddOperator(
            SyntaxKind.LessThanOrEqualOperator,
            new FunctionType([new("x", type), new("y", type)], Bool));
        type.AddOperator(
            SyntaxKind.GreaterThanOperator,
            new FunctionType([new("x", type), new("y", type)], Bool));
        type.AddOperator(
            SyntaxKind.GreaterThanOrEqualOperator,
            new FunctionType([new("x", type), new("y", type)], Bool));
        return type;
    }

    private static PrimType AddLogicalOperators(this PrimType type)
    {
        type.AddOperator(
            SyntaxKind.NotOperator,
            new FunctionType([new("x", type)], Bool));
        type.AddOperator(
            SyntaxKind.LogicalAndOperator,
            new FunctionType([new("x", type), new("y", type)], Bool));
        type.AddOperator(
            SyntaxKind.LogicalOrOperator,
            new FunctionType([new("x", type), new("y", type)], Bool));
        return type;
    }

    private static PrimType AddImplicitConversion(this PrimType type, params ReadOnlySpan<PrimType> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ImplicitKeyword, new FunctionType([new Parameter("x", type)], targetType));
        return type;
    }

    private static PrimType AddExplicitConversion(this PrimType type, params ReadOnlySpan<PrimType> targetTypes)
    {
        foreach (var targetType in targetTypes)
            type.AddConversion(SyntaxKind.ExplicitKeyword, new FunctionType([new Parameter("x", type)], targetType));
        return type;
    }
}

