using System.Diagnostics;
using System.Numerics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Interpretation;

internal sealed class GlobalEvaluatedScope : EvaluatedScope
{
    private static GlobalEvaluatedScope? s_instance;
    public static GlobalEvaluatedScope Instance
    {
        get
        {
            if (s_instance is null)
                Interlocked.CompareExchange(ref s_instance, new GlobalEvaluatedScope(), null);
            return s_instance;
        }
    }

    private GlobalEvaluatedScope() : base()
    {
        var values = MapPredefinedTypes();

        Any = (StructValue)values[Predefined.Any];
        Err = (StructValue)values[Predefined.Err];
        Unknown = (StructValue)values[Predefined.Unknown];
        Never = (StructValue)values[Predefined.Never];
        Unit = (StructValue)values[Predefined.Unit];
        Type = (StructValue)values[Predefined.Type];
        Str = (StructValue)values[Predefined.Str];
        Bool = (StructValue)values[Predefined.Bool];
        I8 = (StructValue)values[Predefined.I8];
        I16 = (StructValue)values[Predefined.I16];
        I32 = (StructValue)values[Predefined.I32];
        I64 = (StructValue)values[Predefined.I64];
        I128 = (StructValue)values[Predefined.I128];
        ISize = (StructValue)values[Predefined.ISize];
        U8 = (StructValue)values[Predefined.U8];
        U16 = (StructValue)values[Predefined.U16];
        U32 = (StructValue)values[Predefined.U32];
        U64 = (StructValue)values[Predefined.U64];
        U128 = (StructValue)values[Predefined.U128];
        USize = (StructValue)values[Predefined.USize];
        F16 = (StructValue)values[Predefined.F16];
        F32 = (StructValue)values[Predefined.F32];
        F64 = (StructValue)values[Predefined.F64];
        F80 = (StructValue)values[Predefined.F80];
        F128 = (StructValue)values[Predefined.F128];

        values[Predefined.Print] = new LambdaValue((LambdaTypeSymbol)Predefined.Print.Type, (PrimValue obj) => { Console.WriteLine(obj.Value); return PrimValue.Unit; });
        values[Predefined.Scan] = new LambdaValue((LambdaTypeSymbol)Predefined.Scan.Type, () => new InstanceValue(Str, Console.ReadLine() ?? ""));

        Values = values;

        static Dictionary<Symbol, PrimValue> MapPredefinedTypes()
        {
            var m = new Dictionary<Symbol, PrimValue>();
            foreach (var symbol in Predefined.All().OfType<StructTypeSymbol>())
            {
                m[symbol] = new StructValue(symbol);
                // TODO: Create members for struct.
            }

            var boolStruct = (StructValue)m[Predefined.Bool];

            ((StructValue)m[Predefined.Err])
                .Add(
                    Predefined.Err.GetProperty("msg") ?? throw new UnreachableException($"Expected property '{"msg"}'"),
                    new InstanceValue(((StructValue)m[Predefined.Str]), ""));

            ((StructValue)m[Predefined.Str])
                .AddEqualityOperators<string>(boolStruct)
                .AddMembers(s =>
                {
                    var add = Predefined.Str.GetBinaryOperators(SyntaxKind.PlusToken, Predefined.Str, Predefined.Str, Predefined.Str).Single();
                    s.Add(
                        add,
                        new LambdaValue(add.LambdaType, (PrimValue a, PrimValue b) =>
                            new InstanceValue(s, (string)a.Value + (string)b.Value)));
                    var addStr = Predefined.Str.GetBinaryOperators(SyntaxKind.PlusToken, Predefined.Str, Predefined.Any, Predefined.Str).Single();
                    s.Add(
                        addStr,
                        new LambdaValue(addStr.LambdaType, (PrimValue a, PrimValue b) =>
                            new InstanceValue(s, (string)a.Value + b.Value)));
                    var addAny = Predefined.Str.GetBinaryOperators(SyntaxKind.PlusToken, Predefined.Any, Predefined.Str, Predefined.Str).Single();
                    s.Add(
                        addAny,
                        new LambdaValue(addAny.LambdaType, (PrimValue a, PrimValue b) =>
                            new InstanceValue(s, a.Value + (string)b.Value)));
                });

            R M<T>(TypeSymbol s) => new((StructValue)m[s], typeof(T));

            ((StructValue)m[Predefined.Bool])
                .AddEqualityOperators<bool>(boolStruct)
                .AddLogicalOperators(boolStruct);
            ((StructValue)m[Predefined.I8])
                .AddEqualityOperators<sbyte>(boolStruct)
                .AddComparisonOperators<sbyte>(boolStruct)
                .AddBitwiseOperators<sbyte>()
                .AddMathOperators<sbyte>()
                .AddImplicitConversion<sbyte>(M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<Half>(Predefined.F16), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<sbyte>(M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128));
            ((StructValue)m[Predefined.I16])
                .AddEqualityOperators<short>(boolStruct)
                .AddComparisonOperators<short>(boolStruct)
                .AddBitwiseOperators<short>()
                .AddMathOperators<short>()
                .AddImplicitConversion<short>(M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<Half>(Predefined.F16), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<short>(M<sbyte>(Predefined.I8), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128));
            ((StructValue)m[Predefined.I32])
                .AddEqualityOperators<int>(boolStruct)
                .AddComparisonOperators<int>(boolStruct)
                .AddBitwiseOperators<int>()
                .AddMathOperators<int>()
                .AddImplicitConversion<int>(M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<int>(M<Half>(Predefined.F16), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128));
            ((StructValue)m[Predefined.I64])
                .AddEqualityOperators<long>(boolStruct)
                .AddComparisonOperators<long>(boolStruct)
                .AddBitwiseOperators<long>()
                .AddMathOperators<long>()
                .AddImplicitConversion<long>(M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<long>(M<Half>(Predefined.F16), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128));
            ((StructValue)m[Predefined.I128])
                .AddEqualityOperators<BigInteger>(boolStruct)
                .AddComparisonOperators<BigInteger>(boolStruct)
                .AddBitwiseOperators<BigInteger>()
                .AddMathOperators<BigInteger>()
                .AddExplicitConversion<BigInteger>(M<Half>(Predefined.F16), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<nint>(Predefined.ISize), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128));
            ((StructValue)m[Predefined.ISize])
                .AddEqualityOperators<nint>(boolStruct)
                .AddComparisonOperators<nint>(boolStruct)
                .AddBitwiseOperators<nint>()
                .AddMathOperators<nint>()
                .AddExplicitConversion<nint>(M<Half>(Predefined.F16), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128), M<nuint>(Predefined.USize));
            ((StructValue)m[Predefined.U8])
                .AddEqualityOperators<byte>(boolStruct)
                .AddComparisonOperators<byte>(boolStruct)
                .AddBitwiseOperators<byte>()
                .AddMathOperators<byte>()
                .AddImplicitConversion<byte>(M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128), M<nuint>(Predefined.USize), M<Half>(Predefined.F16), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<byte>(M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize));
            ((StructValue)m[Predefined.U16])
                .AddEqualityOperators<ushort>(boolStruct)
                .AddComparisonOperators<ushort>(boolStruct)
                .AddBitwiseOperators<ushort>()
                .AddMathOperators<ushort>()
                .AddImplicitConversion<ushort>(M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128), M<nuint>(Predefined.USize), M<Half>(Predefined.F16), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<ushort>(M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<byte>(Predefined.U8));
            ((StructValue)m[Predefined.U32])
                .AddEqualityOperators<uint>(boolStruct)
                .AddComparisonOperators<uint>(boolStruct)
                .AddBitwiseOperators<uint>()
                .AddMathOperators<uint>()
                .AddImplicitConversion<uint>(M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128), M<nuint>(Predefined.USize), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<uint>(M<Half>(Predefined.F16), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<byte>(Predefined.U8), M<ushort>(Predefined.U16));
            ((StructValue)m[Predefined.U64])
                .AddEqualityOperators<ulong>(boolStruct)
                .AddComparisonOperators<ulong>(boolStruct)
                .AddBitwiseOperators<ulong>()
                .AddMathOperators<ulong>()
                .AddImplicitConversion<ulong>(M<BigInteger>(Predefined.U128), M<nuint>(Predefined.USize), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<ulong>(M<Half>(Predefined.F16), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32));
            ((StructValue)m[Predefined.U128])
                .AddEqualityOperators<BigInteger>(boolStruct)
                .AddComparisonOperators<BigInteger>(boolStruct)
                .AddBitwiseOperators<BigInteger>()
                .AddMathOperators<BigInteger>()
                .AddExplicitConversion<BigInteger>(M<Half>(Predefined.F16), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64));
            ((StructValue)m[Predefined.USize])
                .AddEqualityOperators<nuint>(boolStruct)
                .AddComparisonOperators<nuint>(boolStruct)
                .AddBitwiseOperators<nuint>()
                .AddMathOperators<nuint>()
                .AddExplicitConversion<nuint>(M<Half>(Predefined.F16), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128));
            ((StructValue)m[Predefined.F16])
                .AddEqualityOperators<Half>(boolStruct)
                .AddComparisonOperators<Half>(boolStruct)
                .AddMathOperators<Half>()
                .AddImplicitConversion<Half>(M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<Half>(M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128), M<nuint>(Predefined.USize));
            ((StructValue)m[Predefined.F32])
                .AddEqualityOperators<float>(boolStruct)
                .AddComparisonOperators<float>(boolStruct)
                .AddMathOperators<float>()
                .AddImplicitConversion<float>(M<double>(Predefined.F64), M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<float>(M<Half>(Predefined.F16), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128), M<nuint>(Predefined.USize));
            ((StructValue)m[Predefined.F64])
                .AddEqualityOperators<double>(boolStruct)
                .AddComparisonOperators<double>(boolStruct)
                .AddMathOperators<double>()
                .AddImplicitConversion<double>(M<double>(Predefined.F80), M<double>(Predefined.F128))
                .AddExplicitConversion<double>(M<Half>(Predefined.F16), M<float>(Predefined.F32), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128), M<nuint>(Predefined.USize));
            // TODO: Fix these types using double.
            ((StructValue)m[Predefined.F80])
                .AddEqualityOperators<double>(boolStruct)
                .AddComparisonOperators<double>(boolStruct)
                .AddMathOperators<double>()
                .AddImplicitConversion<double>(M<double>(Predefined.F128))
                .AddExplicitConversion<double>(M<Half>(Predefined.F16), M<float>(Predefined.F32), M<double>(Predefined.F64), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128), M<nuint>(Predefined.USize));
            ((StructValue)m[Predefined.F128])
                .AddEqualityOperators<double>(boolStruct)
                .AddComparisonOperators<double>(boolStruct)
                .AddMathOperators<double>()
                .AddExplicitConversion<double>(M<Half>(Predefined.F16), M<float>(Predefined.F32), M<double>(Predefined.F64), M<double>(Predefined.F80), M<sbyte>(Predefined.I8), M<short>(Predefined.I16), M<int>(Predefined.I32), M<long>(Predefined.I64), M<BigInteger>(Predefined.I128), M<nint>(Predefined.ISize), M<byte>(Predefined.U8), M<ushort>(Predefined.U16), M<uint>(Predefined.U32), M<ulong>(Predefined.U64), M<BigInteger>(Predefined.U128), M<nuint>(Predefined.USize));
            return m;
        }
    }

    public StructValue Any { get; }
    public StructValue Unknown { get; }
    public StructValue Err { get; }
    public StructValue Never { get; }
    public StructValue Unit { get; }
    public StructValue Type { get; }
    public StructValue Str { get; }
    public StructValue Bool { get; }
    public StructValue I8 { get; }
    public StructValue I16 { get; }
    public StructValue I32 { get; }
    public StructValue I64 { get; }
    public StructValue I128 { get; }
    public StructValue ISize { get; }
    public StructValue U8 { get; }
    public StructValue U16 { get; }
    public StructValue U32 { get; }
    public StructValue U64 { get; }
    public StructValue U128 { get; }
    public StructValue USize { get; }
    public StructValue F16 { get; }
    public StructValue F32 { get; }
    public StructValue F64 { get; }
    public StructValue F80 { get; }
    public StructValue F128 { get; }
}

file readonly record struct R(StructValue Struct, Type ClrType);

file static class StructValueExtensions
{
    public static StructValue AddMembers(this StructValue s, Action<StructValue> add)
    {
        add(s);
        return s;
    }

    public static StructValue AddMathOperators<T>(this StructValue s) where T : INumber<T>
    {
        var unaryPlus = s.StructType.GetUnaryOperators(SyntaxKind.PlusToken, s.StructType, s.StructType).Single();
        s.Add(
            unaryPlus,
            new LambdaValue(unaryPlus.LambdaType, (PrimValue a) => new InstanceValue(s, +(T)a.Value)));
        var unaryMinus = s.StructType.GetUnaryOperators(SyntaxKind.MinusToken, s.StructType, s.StructType).Single();
        s.Add(
            unaryMinus,
            new LambdaValue(unaryMinus.LambdaType, (PrimValue a) => new InstanceValue(s, -(T)a.Value)));
        var add = s.StructType.GetBinaryOperators(SyntaxKind.PlusToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            add,
            new LambdaValue(add.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, (T)a.Value + (T)b.Value)));
        var subtract = s.StructType.GetBinaryOperators(SyntaxKind.MinusToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            subtract,
            new LambdaValue(subtract.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, (T)a.Value - (T)b.Value)));
        var multiply = s.StructType.GetBinaryOperators(SyntaxKind.StarToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            multiply,
            new LambdaValue(multiply.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, (T)a.Value * (T)b.Value)));
        var divide = s.StructType.GetBinaryOperators(SyntaxKind.SlashToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            divide,
            new LambdaValue(divide.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, (T)a.Value / (T)b.Value)));
        var modulo = s.StructType.GetBinaryOperators(SyntaxKind.PercentToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            modulo,
            new LambdaValue(modulo.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, (T)a.Value % (T)b.Value)));
        var power = s.StructType.GetBinaryOperators(SyntaxKind.StarStarToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            power,
            new LambdaValue(power.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, T.CreateTruncating(Math.Pow(double.CreateTruncating((T)a.Value), double.CreateTruncating((T)b.Value))))));
        return s;
    }

    public static StructValue AddBitwiseOperators<T>(this StructValue s) where T : IBinaryInteger<T>, IShiftOperators<T, int, T>
    {
        var onesComplement = s.StructType.GetUnaryOperators(SyntaxKind.TildeToken, s.StructType, s.StructType).Single();
        s.Add(
            onesComplement,
            new LambdaValue(onesComplement.LambdaType, (PrimValue a) => new InstanceValue(s, ~(T)a.Value)));
        var bitwiseAnd = s.StructType.GetBinaryOperators(SyntaxKind.AmpersandToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            bitwiseAnd,
            new LambdaValue(bitwiseAnd.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, (T)a.Value & (T)b.Value)));
        var bitwiseOr = s.StructType.GetBinaryOperators(SyntaxKind.PipeToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            bitwiseOr,
            new LambdaValue(bitwiseOr.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, (T)a.Value | (T)b.Value)));
        var exclusiveOr = s.StructType.GetBinaryOperators(SyntaxKind.HatToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            exclusiveOr,
            new LambdaValue(exclusiveOr.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, (T)a.Value ^ (T)b.Value)));
        var leftShift = s.StructType.GetBinaryOperators(SyntaxKind.LessThanLessThanToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            leftShift,
            new LambdaValue(leftShift.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, (T)a.Value << int.CreateTruncating((T)b.Value))));
        var rightShift = s.StructType.GetBinaryOperators(SyntaxKind.GreaterThanGreaterThanToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            rightShift,
            new LambdaValue(rightShift.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(s, (T)a.Value >> int.CreateTruncating((T)b.Value))));
        return s;
    }

    public static StructValue AddEqualityOperators<T>(this StructValue s, StructValue boolStruct)
    {
        var equals = s.StructType.GetBinaryOperators(SyntaxKind.EqualsEqualsToken, s.StructType, s.StructType, Predefined.Bool).Single();
        s.Add(
            equals,
            new LambdaValue(equals.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, ((T)a.Value).Equals(b.Value))));
        var notEquals = s.StructType.GetBinaryOperators(SyntaxKind.BangEqualsToken, s.StructType, s.StructType, Predefined.Bool).Single();
        s.Add(
            notEquals,
            new LambdaValue(notEquals.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, !((T)a.Value).Equals(b.Value))));
        return s;
    }

    public static StructValue AddComparisonOperators<T>(this StructValue s, StructValue boolStruct) where T : IComparisonOperators<T, T, bool>
    {
        var lessThan = s.StructType.GetBinaryOperators(SyntaxKind.LessThanToken, s.StructType, s.StructType, Predefined.Bool).Single();
        s.Add(
            lessThan,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, (T)a.Value < (T)b.Value)));
        var lessThanOrEqual = s.StructType.GetBinaryOperators(SyntaxKind.LessThanEqualsToken, s.StructType, s.StructType, Predefined.Bool).Single();
        s.Add(
            lessThanOrEqual,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, (T)a.Value <= (T)b.Value)));
        var greaterThan = s.StructType.GetBinaryOperators(SyntaxKind.GreaterThanToken, s.StructType, s.StructType, Predefined.Bool).Single();
        s.Add(
            greaterThan,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, (T)a.Value > (T)b.Value)));
        var greaterThanOrEqual = s.StructType.GetBinaryOperators(SyntaxKind.GreaterThanEqualsToken, s.StructType, s.StructType, Predefined.Bool).Single();
        s.Add(
            greaterThanOrEqual,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, (T)a.Value >= (T)b.Value)));
        return s;
    }

    public static StructValue AddLogicalOperators(this StructValue s, StructValue boolStruct)
    {
        var not = s.StructType.GetUnaryOperators(SyntaxKind.BangToken, s.StructType, s.StructType).Single();
        s.Add(
            not,
            new LambdaValue(not.LambdaType, (PrimValue a) =>
                new InstanceValue(boolStruct, !(bool)a.Value)));
        var logicalAnd = s.StructType.GetBinaryOperators(SyntaxKind.AmpersandAmpersandToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            logicalAnd,
            new LambdaValue(logicalAnd.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, (bool)a.Value && (bool)b.Value)));
        var logicalOr = s.StructType.GetBinaryOperators(SyntaxKind.PipePipeToken, s.StructType, s.StructType, s.StructType).Single();
        s.Add(
            logicalOr,
            new LambdaValue(logicalOr.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, (bool)a.Value || (bool)b.Value)));
        return s;
    }

    public static StructValue AddImplicitConversion<T>(this StructValue s, params ReadOnlySpan<R> targetTypes)
    {
        foreach (var (targetStruct, targetTypeCLR) in targetTypes)
        {
            var conversion = s.StructType.GetConversion(s.StructType, targetStruct.StructType)
                ?? throw new UnreachableException($"Missing conversion from {s.StructType} to {targetStruct.StructType}");
            s.Add(
                conversion,
                new LambdaValue(conversion.LambdaType, (PrimValue x) =>
                    new InstanceValue(targetStruct, Convert.ChangeType(x.Value, targetTypeCLR))));
        }
        return s;
    }

    public static StructValue AddExplicitConversion<T>(this StructValue s, params ReadOnlySpan<R> targetTypes)
    {
        foreach (var (targetStruct, targetTypeCLR) in targetTypes)
        {
            var conversion = s.StructType.GetConversion(s.StructType, targetStruct.StructType)
                ?? throw new UnreachableException($"Missing conversion from {s.StructType} to {targetStruct.StructType}");
            s.Add(
                conversion,
                new LambdaValue(conversion.LambdaType, (PrimValue x) =>
                    new InstanceValue(targetStruct, Convert.ChangeType(x.Value, targetTypeCLR))));
        }
        return s;
    }
}
