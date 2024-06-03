using System.Reflection;
using System.Runtime.CompilerServices;
using CodeAnalysis.Binding.Types.Metadata;

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
        Str.SetOperators(GetEqualityOperators(Str));

        Bool.SetOperators(GetEqualityOperators(Bool), GetLogicalOperators(Bool));

        I8.SetOperators(GetEqualityOperators(I8), GetComparisonOperators(I8), GetBitwiseOperators(I8), GetMathOperators(I8));
        I16.SetOperators(GetEqualityOperators(I16), GetComparisonOperators(I16), GetBitwiseOperators(I16), GetMathOperators(I16));
        I32.SetOperators(GetEqualityOperators(I32), GetComparisonOperators(I32), GetBitwiseOperators(I32), GetMathOperators(I32));
        I64.SetOperators(GetEqualityOperators(I64), GetComparisonOperators(I64), GetBitwiseOperators(I64), GetMathOperators(I64));
        I128.SetOperators(GetEqualityOperators(I128), GetComparisonOperators(I128), GetBitwiseOperators(I128), GetMathOperators(I128));
        ISize.SetOperators(GetEqualityOperators(ISize), GetComparisonOperators(ISize), GetBitwiseOperators(ISize), GetMathOperators(ISize));

        U8.SetOperators(GetEqualityOperators(U8), GetComparisonOperators(U8), GetBitwiseOperators(U8), GetMathOperators(U8));
        U16.SetOperators(GetEqualityOperators(U16), GetComparisonOperators(U16), GetBitwiseOperators(U16), GetMathOperators(U16));
        U32.SetOperators(GetEqualityOperators(U32), GetComparisonOperators(U32), GetBitwiseOperators(U32), GetMathOperators(U32));
        U64.SetOperators(GetEqualityOperators(U64), GetComparisonOperators(U64), GetBitwiseOperators(U64), GetMathOperators(U64));
        U128.SetOperators(GetEqualityOperators(U128), GetComparisonOperators(U128), GetBitwiseOperators(U128), GetMathOperators(U128));
        USize.SetOperators(GetEqualityOperators(USize), GetComparisonOperators(USize), GetBitwiseOperators(USize), GetMathOperators(USize));

        F16.SetOperators(GetEqualityOperators(F16), GetComparisonOperators(F16), GetMathOperators(F16));
        F32.SetOperators(GetEqualityOperators(F32), GetComparisonOperators(F32), GetMathOperators(F32));
        F64.SetOperators(GetEqualityOperators(F64), GetComparisonOperators(F64), GetMathOperators(F64));
        F80.SetOperators(GetEqualityOperators(F80), GetComparisonOperators(F80), GetMathOperators(F80));
        F128.SetOperators(GetEqualityOperators(F128), GetComparisonOperators(F128), GetMathOperators(F128));
    }

    public static ReadOnlyList<PredefinedType> All { get; } = new(typeof(PredefinedTypes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(PredefinedType))
        .Select(f => (PredefinedType)f.GetValue(null)!)
        .ToArray());

    private static List<Operator> GetMathOperators(PrimType type)
    {
        return [
            new Operator("+", new FunctionType([new("x", type)], type)),
            new Operator("-", new FunctionType([new("x", type)], type)),
            new Operator("++", new FunctionType([new("x", type)], type)),
            new Operator("--", new FunctionType([new("x", type)], type)),
            new Operator("+", new FunctionType([new("x", type), new("y", type)], type)),
            new Operator("-", new FunctionType([new("x", type), new("y", type)], type)),
            new Operator("*", new FunctionType([new("x", type), new("y", type)], type)),
            new Operator("/", new FunctionType([new("x", type), new("y", type)], type)),
            new Operator("%", new FunctionType([new("x", type), new("y", type)], type)),
            new Operator("**", new FunctionType([new("x", type), new("y", type)], type))
         ];
    }

    private static List<Operator> GetBitwiseOperators(PrimType type)
    {
        return [
            new Operator("~", new FunctionType([new("x", type)], type)),
            new Operator("&", new FunctionType([new("x", type), new("y", type)], type)),
            new Operator("|", new FunctionType([new("x", type), new("y", type)], type)),
            new Operator("^", new FunctionType([new("x", type), new("y", type)], type)),
            new Operator("<<", new FunctionType([new("x", type), new("y", type)], type)),
            new Operator(">>", new FunctionType([new("x", type), new("y", type)], type)),
        ];
    }

    private static List<Operator> GetEqualityOperators(PrimType type)
    {
        return [
            new Operator("==", new FunctionType([new("x", type), new("y", type)], Bool)),
            new Operator("!=", new FunctionType([new("x", type), new("y", type)], Bool)),
        ];
    }

    private static List<Operator> GetComparisonOperators(PrimType type)
    {
        return [
            new Operator("<", new FunctionType([new("x", type), new("y", type)], Bool)),
            new Operator("<=", new FunctionType([new("x", type), new("y", type)], Bool)),
            new Operator(">", new FunctionType([new("x", type), new("y", type)], Bool)),
            new Operator(">=", new FunctionType([new("x", type), new("y", type)], Bool)),
        ];
    }

    private static List<Operator> GetLogicalOperators(PrimType type)
    {
        return [
            new Operator("!", new FunctionType([new("x", type)], Bool)),
            new Operator("&&", new FunctionType([new("x", type), new("y", type)], Bool)),
            new Operator("||", new FunctionType([new("x", type), new("y", type)], Bool)),
        ];
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

file static class PrimTypeExtensions
{
    public static void SetOperators(this PrimType type, params List<List<Operator>> operators)
    {
        SetOperators(type, [.. operators.SelectMany(x => x)]);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Operators")]
        static extern void SetOperators(PrimType type, ReadOnlyList<Operator> operators);
    }
}

