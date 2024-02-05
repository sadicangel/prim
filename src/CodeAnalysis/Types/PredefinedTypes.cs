using CodeAnalysis.Operators;
using System.Reflection;

namespace CodeAnalysis.Types;
internal static class PredefinedTypes
{
    public static readonly PredefinedType Any = new(PredefinedSymbolNames.Any);
    public static readonly PredefinedType Unknown = new(PredefinedSymbolNames.Unknown);
    public static readonly PredefinedType Never = new(PredefinedSymbolNames.Never);
    public static readonly PredefinedType Unit = new(PredefinedSymbolNames.Unit);
    public static readonly PredefinedType Type = new(PredefinedSymbolNames.Type);
    public static readonly PredefinedType Str = new(PredefinedSymbolNames.Str);
    public static readonly PredefinedType Bool = new(PredefinedSymbolNames.Bool);
    public static readonly PredefinedType I8 = new(PredefinedSymbolNames.I8);
    public static readonly PredefinedType I16 = new(PredefinedSymbolNames.I16);
    public static readonly PredefinedType I32 = new(PredefinedSymbolNames.I32);
    public static readonly PredefinedType I64 = new(PredefinedSymbolNames.I64);
    public static readonly PredefinedType I128 = new(PredefinedSymbolNames.I128);
    public static readonly PredefinedType ISize = new(PredefinedSymbolNames.ISize);
    public static readonly PredefinedType U8 = new(PredefinedSymbolNames.U8);
    public static readonly PredefinedType U16 = new(PredefinedSymbolNames.U16);
    public static readonly PredefinedType U32 = new(PredefinedSymbolNames.U32);
    public static readonly PredefinedType U64 = new(PredefinedSymbolNames.U64);
    public static readonly PredefinedType U128 = new(PredefinedSymbolNames.U128);
    public static readonly PredefinedType USize = new(PredefinedSymbolNames.USize);
    public static readonly PredefinedType F16 = new(PredefinedSymbolNames.F16);
    public static readonly PredefinedType F32 = new(PredefinedSymbolNames.F32);
    public static readonly PredefinedType F64 = new(PredefinedSymbolNames.F64);
    public static readonly PredefinedType F80 = new(PredefinedSymbolNames.F80);
    public static readonly PredefinedType F128 = new(PredefinedSymbolNames.F128);

    static PredefinedTypes()
    {
        Str.AddEqualityOperators();

        Bool.AddEqualityOperators().AddLogicalOperators();

        PredefinedType[] integers = [I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize];
        foreach (var integer in integers)
            integer.AddMathOperators().AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators();

        PredefinedType[] floats = [F16, F32, F64, F80, F128];
        foreach (var @float in floats)
            @float.AddMathOperators().AddEqualityOperators().AddComparisonOperators();
    }

    public static IReadOnlyList<PredefinedType> All { get; } = typeof(PredefinedTypes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(PredefinedType))
        .Select(f => (PredefinedType)f.GetValue(null)!)
        .ToArray();
}

internal static class PrimTypeExtensions
{
    public static T AddMathOperators<T>(this T type) where T : PrimType
    {
        type.Operators.AddRange([
            new UnaryOperator(OperatorKind.UnaryPlus, type, type),
            new UnaryOperator(OperatorKind.Negate, type, type),
            new UnaryOperator(OperatorKind.Increment, type, type),
            new UnaryOperator(OperatorKind.Decrement, type, type),
            new BinaryOperator(OperatorKind.Add, type, type, type),
            new BinaryOperator(OperatorKind.Subtract, type, type, type),
            new BinaryOperator(OperatorKind.Multiply, type, type, type),
            new BinaryOperator(OperatorKind.Divide, type, type, type),
            new BinaryOperator(OperatorKind.Modulo, type, type, type),
            new BinaryOperator(OperatorKind.Exponent, type, type, type)
        ]);
        return type;
    }

    public static T AddBitwiseOperators<T>(this T type) where T : PrimType
    {
        type.Operators.AddRange([
            new UnaryOperator(OperatorKind.OnesComplement, type, type),
            new BinaryOperator(OperatorKind.And, type, type, type),
            new BinaryOperator(OperatorKind.Or, type, type, type),
            new BinaryOperator(OperatorKind.ExclusiveOr, type, type, type),
            new BinaryOperator(OperatorKind.LeftShift, type, type, type),
            new BinaryOperator(OperatorKind.RightShift, type, type, type)
        ]);
        return type;
    }

    public static T AddEqualityOperators<T>(this T type) where T : PrimType
    {
        type.Operators.AddRange([
            new BinaryOperator(OperatorKind.Equal, type, type, PredefinedTypes.Bool),
            new BinaryOperator(OperatorKind.NotEqual, type, type, PredefinedTypes.Bool),
        ]);
        return type;
    }

    public static T AddComparisonOperators<T>(this T type) where T : PrimType
    {
        type.Operators.AddRange([
            new BinaryOperator(OperatorKind.LessThan, type, type, PredefinedTypes.Bool),
            new BinaryOperator(OperatorKind.LessThanOrEqual, type, type, PredefinedTypes.Bool),
            new BinaryOperator(OperatorKind.GreaterThan, type, type, PredefinedTypes.Bool),
            new BinaryOperator(OperatorKind.GreaterThanOrEqual, type, type, PredefinedTypes.Bool),
        ]);
        return type;
    }

    public static T AddLogicalOperators<T>(this T type) where T : PrimType
    {
        type.Operators.AddRange([
            new UnaryOperator(OperatorKind.Not, type, PredefinedTypes.Bool),
            new BinaryOperator(OperatorKind.AndAlso, type, type, PredefinedTypes.Bool),
            new BinaryOperator(OperatorKind.OrElse, type, type, PredefinedTypes.Bool)
        ]);
        return type;
    }

    public static FunctionType AddCallOperator(this FunctionType type)
    {
        type.Operators.Add(new BinaryOperator(
            OperatorKind.Call,
            type,
            new TypeList([.. type.Parameters.Select(p => p.Type)]),
            type.ReturnType));
        return type;
    }
}