using CodeAnalysis.Operators;
using System.Reflection;

namespace CodeAnalysis.Types;
internal static class PredefinedTypes
{
    public static readonly PredefinedType Any = new(PredefinedTypeNames.Any);
    public static readonly PredefinedType Unknown = new(PredefinedTypeNames.Unknown);
    public static readonly PredefinedType Never = new(PredefinedTypeNames.Never);
    public static readonly PredefinedType Unit = new(PredefinedTypeNames.Unit);
    public static readonly PredefinedType Type = new(PredefinedTypeNames.Type);
    public static readonly PredefinedType Str = new(PredefinedTypeNames.Str);
    public static readonly PredefinedType Bool = new(PredefinedTypeNames.Bool);
    public static readonly PredefinedType I8 = new(PredefinedTypeNames.I8);
    public static readonly PredefinedType I16 = new(PredefinedTypeNames.I16);
    public static readonly PredefinedType I32 = new(PredefinedTypeNames.I32);
    public static readonly PredefinedType I64 = new(PredefinedTypeNames.I64);
    public static readonly PredefinedType I128 = new(PredefinedTypeNames.I128);
    public static readonly PredefinedType ISize = new(PredefinedTypeNames.ISize);
    public static readonly PredefinedType U8 = new(PredefinedTypeNames.U8);
    public static readonly PredefinedType U16 = new(PredefinedTypeNames.U16);
    public static readonly PredefinedType U32 = new(PredefinedTypeNames.U32);
    public static readonly PredefinedType U64 = new(PredefinedTypeNames.U64);
    public static readonly PredefinedType U128 = new(PredefinedTypeNames.U128);
    public static readonly PredefinedType USize = new(PredefinedTypeNames.USize);
    public static readonly PredefinedType F16 = new(PredefinedTypeNames.F16);
    public static readonly PredefinedType F32 = new(PredefinedTypeNames.F32);
    public static readonly PredefinedType F64 = new(PredefinedTypeNames.F64);
    public static readonly PredefinedType F80 = new(PredefinedTypeNames.F80);
    public static readonly PredefinedType F128 = new(PredefinedTypeNames.F128);

    static PredefinedTypes()
    {
        PredefinedType[] integers = [I8, I16, I32, I64, I128, ISize, U8, U16, U32, U64, U128, USize];
        foreach (var integer in integers)
            integer.AddMathOperators().AddEqualityOperators().AddComparisonOperators().AddShiftOperators().AddLogicalOperators();
    }

    public static IReadOnlyList<PredefinedType> All { get; } = typeof(PredefinedTypes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(PredefinedType))
        .Select(f => (PredefinedType)f.GetValue(null)!)
        .ToArray();
}

internal static class PredefinedTypeNames
{
    public const string Any = "any";
    public const string Unknown = "unknown";
    public const string Never = "never";
    public const string Unit = "unit";
    public const string Type = "type";
    public const string Str = "str";
    public const string Bool = "bool";
    public const string I8 = "i8";
    public const string I16 = "i16";
    public const string I32 = "i32";
    public const string I64 = "i64";
    public const string I128 = "i128";
    public const string ISize = "isize";
    public const string U8 = "u8";
    public const string U16 = "u16";
    public const string U32 = "u32";
    public const string U64 = "u64";
    public const string U128 = "u128";
    public const string USize = "usize";
    public const string F16 = "f16";
    public const string F32 = "f32";
    public const string F64 = "f64";
    public const string F80 = "f80";
    public const string F128 = "f128";

    public static IReadOnlyList<string> All { get; } = typeof(PredefinedTypeNames)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral && f.IsInitOnly)
        .Select(f => (string)f.GetValue(null)!)
        .ToArray();
}

file static class PredefinedTypesHelper
{
    public static PredefinedType AddMathOperators(this PredefinedType type)
    {
        type.BinaryOperators.AddRange([
            new BinaryOperator(BinaryOperatorKind.Add, type, type, type),
            new BinaryOperator(BinaryOperatorKind.Subtract, type, type, type),
            new BinaryOperator(BinaryOperatorKind.Multiply, type, type, type),
            new BinaryOperator(BinaryOperatorKind.Divide, type, type, type),
            new BinaryOperator(BinaryOperatorKind.Modulo, type, type, type),
            new BinaryOperator(BinaryOperatorKind.Exponent, type, type, type)
        ]);
        return type;
    }

    public static PredefinedType AddShiftOperators(this PredefinedType type)
    {
        type.BinaryOperators.AddRange([
            new BinaryOperator(BinaryOperatorKind.LeftShift, type, type, type),
            new BinaryOperator(BinaryOperatorKind.RightShift, type, type, type)
        ]);
        return type;
    }

    public static PredefinedType AddLogicalOperators(this PredefinedType type)
    {
        type.BinaryOperators.AddRange([
            new BinaryOperator(BinaryOperatorKind.And, type, type, type),
            new BinaryOperator(BinaryOperatorKind.Or, type, type, type),
            new BinaryOperator(BinaryOperatorKind.ExclusiveOr, type, type, type),
        ]);
        return type;
    }

    public static PredefinedType AddEqualityOperators(this PredefinedType type)
    {
        type.BinaryOperators.AddRange([
            new BinaryOperator(BinaryOperatorKind.Equal, type, type, PredefinedTypes.Bool),
            new BinaryOperator(BinaryOperatorKind.NotEqual, type, type, PredefinedTypes.Bool),
        ]);
        return type;
    }

    public static PredefinedType AddComparisonOperators(this PredefinedType type)
    {
        type.BinaryOperators.AddRange([
            new BinaryOperator(BinaryOperatorKind.LessThan, type, type, PredefinedTypes.Bool),
            new BinaryOperator(BinaryOperatorKind.LessThanOrEqual, type, type, PredefinedTypes.Bool),
            new BinaryOperator(BinaryOperatorKind.GreaterThan, type, type, PredefinedTypes.Bool),
            new BinaryOperator(BinaryOperatorKind.GreaterThanOrEqual, type, type, PredefinedTypes.Bool),
        ]);
        return type;
    }
}