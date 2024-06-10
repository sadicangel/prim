using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Types;
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
        Str.AddEqualityOperators();

        Bool.AddEqualityOperators().AddLogicalOperators();

        I8.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();
        I16.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();
        I32.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();
        I64.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();
        I128.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();
        ISize.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();

        U8.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();
        U16.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();
        U32.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();
        U64.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();
        U128.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();
        USize.AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators().AddMathOperators();

        F16.AddEqualityOperators().AddComparisonOperators().AddMathOperators();
        F32.AddEqualityOperators().AddComparisonOperators().AddMathOperators();
        F64.AddEqualityOperators().AddComparisonOperators().AddMathOperators();
        F80.AddEqualityOperators().AddComparisonOperators().AddMathOperators();
        F128.AddEqualityOperators().AddComparisonOperators().AddMathOperators();
    }

    private static PrimType AddMathOperators(this PrimType type)
    {
        type.AddOperator(new Operator(
            SyntaxKind.UnaryPlusOperator,
            new FunctionType([new("x", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.UnaryMinusOperator,
            new FunctionType([new("x", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.AddOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.SubtractOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.MultiplyOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.DivideOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.ModuloOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.PowerOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        return type;
    }

    private static PrimType AddBitwiseOperators(this PrimType type)
    {
        type.AddOperator(new Operator(
            SyntaxKind.OnesComplementOperator,
            new FunctionType([new("x", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.BitwiseAndOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.BitwiseOrOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.ExclusiveOrOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.LeftShiftOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        type.AddOperator(new Operator(
            SyntaxKind.RightShiftOperator,
            new FunctionType([new("x", type), new("y", type)], type)));
        return type;
    }

    private static PrimType AddEqualityOperators(this PrimType type)
    {
        type.AddOperator(new Operator(
            SyntaxKind.EqualsOperator,
            new FunctionType([new("x", type), new("y", type)], Bool)));
        type.AddOperator(new Operator(
            SyntaxKind.NotEqualsOperator,
            new FunctionType([new("x", type), new("y", type)], Bool)));
        return type;
    }

    private static PrimType AddComparisonOperators(this PrimType type)
    {
        type.AddOperator(new Operator(
            SyntaxKind.LessThanOperator,
            new FunctionType([new("x", type), new("y", type)], Bool)));
        type.AddOperator(new Operator(
            SyntaxKind.LessThanOrEqualOperator,
            new FunctionType([new("x", type), new("y", type)], Bool)));
        type.AddOperator(new Operator(
            SyntaxKind.GreaterThanOperator,
            new FunctionType([new("x", type), new("y", type)], Bool)));
        type.AddOperator(new Operator(
            SyntaxKind.GreaterThanOrEqualOperator,
            new FunctionType([new("x", type), new("y", type)], Bool)));
        return type;
    }

    private static PrimType AddLogicalOperators(this PrimType type)
    {
        type.AddOperator(new Operator(
            SyntaxKind.NotOperator,
            new FunctionType([new("x", type)], Bool)));
        type.AddOperator(new Operator(
            SyntaxKind.LogicalAndOperator,
            new FunctionType([new("x", type), new("y", type)], Bool)));
        type.AddOperator(new Operator(
            SyntaxKind.LogicalOrOperator,
            new FunctionType([new("x", type), new("y", type)], Bool)));
        return type;
    }
}

