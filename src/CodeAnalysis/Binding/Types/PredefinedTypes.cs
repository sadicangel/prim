using System.Reflection;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Types;
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
        Str.Members.AddRange(GetEqualityOperators(Str));

        Bool.Members.AddRange(GetEqualityOperators(Bool).Concat(GetLogicalOperators(Bool)));

        I8.Members.AddRange(GetEqualityOperators(I8).Concat(GetComparisonOperators(I8)).Concat(GetBitwiseOperators(I8)).Concat(GetMathOperators(I8)));
        I16.Members.AddRange(GetEqualityOperators(I16).Concat(GetComparisonOperators(I16)).Concat(GetBitwiseOperators(I16)).Concat(GetMathOperators(I16)));
        I32.Members.AddRange(GetEqualityOperators(I32).Concat(GetComparisonOperators(I32)).Concat(GetBitwiseOperators(I32)).Concat(GetMathOperators(I32)));
        I64.Members.AddRange(GetEqualityOperators(I64).Concat(GetComparisonOperators(I64)).Concat(GetBitwiseOperators(I64)).Concat(GetMathOperators(I64)));
        I128.Members.AddRange(GetEqualityOperators(I128).Concat(GetComparisonOperators(I128)).Concat(GetBitwiseOperators(I128)).Concat(GetMathOperators(I128)));
        ISize.Members.AddRange(GetEqualityOperators(ISize).Concat(GetComparisonOperators(ISize)).Concat(GetBitwiseOperators(ISize)).Concat(GetMathOperators(ISize)));

        U8.Members.AddRange(GetEqualityOperators(U8).Concat(GetComparisonOperators(U8)).Concat(GetBitwiseOperators(U8)).Concat(GetMathOperators(U8)));
        U16.Members.AddRange(GetEqualityOperators(U16).Concat(GetComparisonOperators(U16)).Concat(GetBitwiseOperators(U16)).Concat(GetMathOperators(U16)));
        U32.Members.AddRange(GetEqualityOperators(U32).Concat(GetComparisonOperators(U32)).Concat(GetBitwiseOperators(U32)).Concat(GetMathOperators(U32)));
        U64.Members.AddRange(GetEqualityOperators(U64).Concat(GetComparisonOperators(U64)).Concat(GetBitwiseOperators(U64)).Concat(GetMathOperators(U64)));
        U128.Members.AddRange(GetEqualityOperators(U128).Concat(GetComparisonOperators(U128)).Concat(GetBitwiseOperators(U128)).Concat(GetMathOperators(U128)));
        USize.Members.AddRange(GetEqualityOperators(USize).Concat(GetComparisonOperators(USize)).Concat(GetBitwiseOperators(USize)).Concat(GetMathOperators(USize)));

        F16.Members.AddRange(GetEqualityOperators(F16).Concat(GetComparisonOperators(F16)).Concat(GetMathOperators(F16)));
        F32.Members.AddRange(GetEqualityOperators(F32).Concat(GetComparisonOperators(F32)).Concat(GetMathOperators(F32)));
        F64.Members.AddRange(GetEqualityOperators(F64).Concat(GetComparisonOperators(F64)).Concat(GetMathOperators(F64)));
        F80.Members.AddRange(GetEqualityOperators(F80).Concat(GetComparisonOperators(F80)).Concat(GetMathOperators(F80)));
        F128.Members.AddRange(GetEqualityOperators(F128).Concat(GetComparisonOperators(F128)).Concat(GetMathOperators(F128)));
    }

    public static ReadOnlyList<PredefinedType> All { get; } = new(typeof(PredefinedTypes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(PredefinedType))
        .Select(f => (PredefinedType)f.GetValue(null)!)
        .ToArray());

    private static IEnumerable<Operator> GetMathOperators(PrimType type)
    {
        yield return new Operator(SyntaxKind.UnaryPlusOperator, new FunctionType([new("x", type)], type));
        yield return new Operator(SyntaxKind.UnaryMinusOperator, new FunctionType([new("x", type)], type));
        yield return new Operator(SyntaxKind.PrefixIncrementOperator, new FunctionType([new("x", type)], type));
        yield return new Operator(SyntaxKind.PrefixDecrementOperator, new FunctionType([new("x", type)], type));
        yield return new Operator(SyntaxKind.AddOperator, new FunctionType([new("x", type), new("y", type)], type));
        yield return new Operator(SyntaxKind.SubtractOperator, new FunctionType([new("x", type), new("y", type)], type));
        yield return new Operator(SyntaxKind.MultiplyOperator, new FunctionType([new("x", type), new("y", type)], type));
        yield return new Operator(SyntaxKind.DivideOperator, new FunctionType([new("x", type), new("y", type)], type));
        yield return new Operator(SyntaxKind.ModuloOperator, new FunctionType([new("x", type), new("y", type)], type));
        yield return new Operator(SyntaxKind.PowerOperator, new FunctionType([new("x", type), new("y", type)], type));
    }

    private static IEnumerable<Operator> GetBitwiseOperators(PrimType type)
    {
        yield return new Operator(SyntaxKind.OnesComplementOperator, new FunctionType([new("x", type)], type));
        yield return new Operator(SyntaxKind.BitwiseAndOperator, new FunctionType([new("x", type), new("y", type)], type));
        yield return new Operator(SyntaxKind.BitwiseOrOperator, new FunctionType([new("x", type), new("y", type)], type));
        yield return new Operator(SyntaxKind.ExclusiveOrOperator, new FunctionType([new("x", type), new("y", type)], type));
        yield return new Operator(SyntaxKind.LeftShiftOperator, new FunctionType([new("x", type), new("y", type)], type));
        yield return new Operator(SyntaxKind.RightShiftOperator, new FunctionType([new("x", type), new("y", type)], type));
    }

    private static IEnumerable<Operator> GetEqualityOperators(PrimType type)
    {
        yield return new Operator(SyntaxKind.EqualsOperator, new FunctionType([new("x", type), new("y", type)], Bool));
        yield return new Operator(SyntaxKind.NotEqualsOperator, new FunctionType([new("x", type), new("y", type)], Bool));
    }

    private static IEnumerable<Operator> GetComparisonOperators(PrimType type)
    {
        yield return new Operator(SyntaxKind.LessThanOperator, new FunctionType([new("x", type), new("y", type)], Bool));
        yield return new Operator(SyntaxKind.LessThanOrEqualOperator, new FunctionType([new("x", type), new("y", type)], Bool));
        yield return new Operator(SyntaxKind.GreaterThanOperator, new FunctionType([new("x", type), new("y", type)], Bool));
        yield return new Operator(SyntaxKind.GreaterThanOrEqualOperator, new FunctionType([new("x", type), new("y", type)], Bool));
    }

    private static IEnumerable<Operator> GetLogicalOperators(PrimType type)
    {
        yield return new Operator(SyntaxKind.NotOperator, new FunctionType([new("x", type)], Bool));
        yield return new Operator(SyntaxKind.LogicalAndOperator, new FunctionType([new("x", type), new("y", type)], Bool));
        yield return new Operator(SyntaxKind.LogicalOrOperator, new FunctionType([new("x", type), new("y", type)], Bool));
    }

    //public static T AddExplicitConversionOperators(PrimType type)
    //{
    //    type.Operators.AddRange(types.Select(other => new Operator(ExplicitConversion, type, other)));
    //    return type;
    //}

    //public static T AddImplicitConversionOperators(PrimType type)
    //{
    //    type.Operators.AddRange(types.Select(other => new Operator(ImplicitConversion, type, other)));
    //    return type;
    //}
}

