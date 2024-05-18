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
        // Strings.
        Str.AddEqualityOperators();

        // Booleans.
        Bool.AddEqualityOperators().AddLogicalOperators();

        // Numbers.
        PredefinedType[] ints = [I8, I16, I32, I64, I128];
        PredefinedType[] uints = [U8, U16, U32, U64, U128];
        PredefinedType[] floats = [F16, F32, F64, F80, F128];

        // Common number operations.
        foreach (var @int in ints)
            @int.AddMathOperators().AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators();
        foreach (var @uint in uints)
            @uint.AddMathOperators().AddEqualityOperators().AddComparisonOperators().AddBitwiseOperators();
        foreach (var @float in floats)
            @float.AddMathOperators().AddEqualityOperators().AddComparisonOperators();

        // Implicit conversions: smaller -> bigger.
        for (int i = 0; i < ints.Length - 1; ++i)
            ints[i].AddImplicitConversionOperators(ints[(i + 1)..]);
        for (int i = 0; i < uints.Length - 1; ++i)
            uints[i].AddImplicitConversionOperators(ints[(i + 1)..]);
        for (int i = 0; i < floats.Length - 1; ++i)
            floats[i].AddImplicitConversionOperators(floats[(i + 1)..]);

        // Implicit conversions: uint -> int.
        for (int i = 0; i < uints.Length - 1; ++i)
            uints[i].AddImplicitConversionOperators(ints[(i + 1)..]);

        // Implicit conversions: int/uint -> float.
        I8.AddImplicitConversionOperators(F16, F32, F64, F80, F128);
        U8.AddImplicitConversionOperators(F16, F32, F64, F80, F128);
        I16.AddImplicitConversionOperators(F32, F64, F80, F128);
        U16.AddImplicitConversionOperators(F32, F64, F80, F128);
        I32.AddImplicitConversionOperators(F32, F64, F80, F128);
        U32.AddImplicitConversionOperators(F32, F64, F80, F128);
        I64.AddImplicitConversionOperators(F32, F64, F80, F128);
        U64.AddImplicitConversionOperators(F32, F64, F80, F128);
        I128.AddImplicitConversionOperators(F80, F128);
        U128.AddImplicitConversionOperators(F80, F128);

        // Explicit conversions: bigger -> smaller.
        for (int i = ints.Length - 1; i > 0; --i)
            ints[i].AddExplicitConversionOperators(ints[..i]);
        for (int i = uints.Length - 1; i > 0; --i)
            uints[i].AddExplicitConversionOperators([.. ints, .. uints[..i], .. floats]);
        for (int i = floats.Length - 1; i > 0; --i)
            floats[i].AddExplicitConversionOperators(floats[..i]);

        // Explicit conversions: int -> uint.
        for (int i = 0; i < ints.Length; ++i)
            ints[i].AddExplicitConversionOperators(uints);

        // Explicit conversions: uint -> int.
        for (int i = uints.Length - 1; i > 0; --i)
            uints[i].AddExplicitConversionOperators(ints[..i]);

        // Explicit conversions: int/uint -> float.
        I16.AddExplicitConversionOperators(F16);
        U16.AddExplicitConversionOperators(F16);
        I32.AddExplicitConversionOperators(F16);
        U32.AddExplicitConversionOperators(F16);
        I64.AddExplicitConversionOperators(F16);
        U64.AddExplicitConversionOperators(F16);
        I128.AddExplicitConversionOperators(F16, F32, F64);
        U128.AddExplicitConversionOperators(F16, F32, F64);

        // Explicit conversions: float -> int/uint.
        for (int i = 0; i < floats.Length; ++i)
            floats[i].AddExplicitConversionOperators([.. ints, .. uints]);

        // Pointers: ISize.
        ISize.AddMathOperators().AddEqualityOperators().AddComparisonOperators();
        I8.AddImplicitConversionOperators(ISize);
        U8.AddImplicitConversionOperators(ISize);
        I16.AddImplicitConversionOperators(ISize);
        U16.AddImplicitConversionOperators(ISize);
        I32.AddImplicitConversionOperators(ISize);
        if (nint.Size == 8)
        {
            U32.AddImplicitConversionOperators(ISize);
            I64.AddImplicitConversionOperators(ISize);
        }
        else
        {
            U32.AddExplicitConversionOperators(ISize);
            I64.AddExplicitConversionOperators(ISize);
        }
        U64.AddImplicitConversionOperators(ISize);
        I128.AddImplicitConversionOperators(ISize);
        U128.AddImplicitConversionOperators(ISize);

        // Pointers: IUSize.
        USize.AddMathOperators().AddEqualityOperators().AddComparisonOperators();
        U8.AddImplicitConversionOperators(USize);
        U16.AddImplicitConversionOperators(USize);
        U32.AddImplicitConversionOperators(USize);
        if (nuint.Size == 8)
        {
            U64.AddImplicitConversionOperators(USize);
        }
        else
        {
            U64.AddExplicitConversionOperators(USize);
        }
        I8.AddImplicitConversionOperators(USize);
        I16.AddImplicitConversionOperators(USize);
        I32.AddImplicitConversionOperators(USize);
        I64.AddImplicitConversionOperators(USize);
        I128.AddImplicitConversionOperators(USize);
        U128.AddImplicitConversionOperators(USize);
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
            new UnaryOperatorInfo(OperatorKind.UnaryPlus, type, type),
            new UnaryOperatorInfo(OperatorKind.Negate, type, type),
            new UnaryOperatorInfo(OperatorKind.PrefixIncrement, type, type),
            new UnaryOperatorInfo(OperatorKind.PrefixDecrement, type, type),
            new BinaryOperatorInfo(OperatorKind.Add, type, type, type),
            new BinaryOperatorInfo(OperatorKind.Subtract, type, type, type),
            new BinaryOperatorInfo(OperatorKind.Multiply, type, type, type),
            new BinaryOperatorInfo(OperatorKind.Divide, type, type, type),
            new BinaryOperatorInfo(OperatorKind.Modulo, type, type, type),
            new BinaryOperatorInfo(OperatorKind.Exponent, type, type, type)
        ]);
        return type;
    }

    public static T AddBitwiseOperators<T>(this T type) where T : PrimType
    {
        type.Operators.AddRange([
            new UnaryOperatorInfo(OperatorKind.OnesComplement, type, type),
            new BinaryOperatorInfo(OperatorKind.And, type, type, type),
            new BinaryOperatorInfo(OperatorKind.Or, type, type, type),
            new BinaryOperatorInfo(OperatorKind.ExclusiveOr, type, type, type),
            new BinaryOperatorInfo(OperatorKind.LeftShift, type, type, type),
            new BinaryOperatorInfo(OperatorKind.RightShift, type, type, type)
        ]);
        return type;
    }

    public static T AddEqualityOperators<T>(this T type) where T : PrimType
    {
        type.Operators.AddRange([
            new BinaryOperatorInfo(OperatorKind.Equal, type, type, PredefinedTypes.Bool),
            new BinaryOperatorInfo(OperatorKind.NotEqual, type, type, PredefinedTypes.Bool),
        ]);
        return type;
    }

    public static T AddComparisonOperators<T>(this T type) where T : PrimType
    {
        type.Operators.AddRange([
            new BinaryOperatorInfo(OperatorKind.LessThan, type, type, PredefinedTypes.Bool),
            new BinaryOperatorInfo(OperatorKind.LessThanOrEqual, type, type, PredefinedTypes.Bool),
            new BinaryOperatorInfo(OperatorKind.GreaterThan, type, type, PredefinedTypes.Bool),
            new BinaryOperatorInfo(OperatorKind.GreaterThanOrEqual, type, type, PredefinedTypes.Bool),
        ]);
        return type;
    }

    public static T AddLogicalOperators<T>(this T type) where T : PrimType
    {
        type.Operators.AddRange([
            new UnaryOperatorInfo(OperatorKind.Not, type, PredefinedTypes.Bool),
            new BinaryOperatorInfo(OperatorKind.AndAlso, type, type, PredefinedTypes.Bool),
            new BinaryOperatorInfo(OperatorKind.OrElse, type, type, PredefinedTypes.Bool)
        ]);
        return type;
    }

    public static T AddExplicitConversionOperators<T>(this T type, params PrimType[] types) where T : PrimType
    {
        type.Operators.AddRange(types.Select(other => new UnaryOperatorInfo(OperatorKind.ExplicitConversion, type, other)));
        return type;
    }

    public static T AddImplicitConversionOperators<T>(this T type, params PrimType[] types) where T : PrimType
    {
        type.Operators.AddRange(types.Select(other => new UnaryOperatorInfo(OperatorKind.ImplicitConversion, type, other)));
        return type;
    }
}