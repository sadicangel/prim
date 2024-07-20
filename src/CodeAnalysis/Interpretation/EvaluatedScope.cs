using System.Collections;
using System.Diagnostics;
using System.Numerics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Interpretation;

internal class EvaluatedScope(EvaluatedScope? parent = null) : IEnumerable<PrimValue>
{
    protected Dictionary<Symbol, PrimValue>? Values { get; set; }

    public EvaluatedScope Parent { get => parent ?? GlobalEvaluatedScope.Instance; }

    public void Declare(Symbol symbol, PrimValue value, bool @throw = true)
    {
        if (!(Values ??= []).TryAdd(symbol, value) && @throw)
            throw new UnreachableException(DiagnosticMessage.SymbolRedeclaration(symbol.Name));
    }

    public void Replace(Symbol symbol, PrimValue value)
    {
        var scope = this;
        if (scope.Values?.ContainsKey(symbol) is true)
        {
            scope.Values[symbol] = value;
            return;
        }

        do
        {
            scope = scope.Parent;
            if (scope.Values?.ContainsKey(symbol) is true)
            {
                scope.Values[symbol] = value;
                return;
            }
        }
        while (scope != scope.Parent);

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public PrimValue Lookup(Symbol symbol)
    {
        var scope = this;
        var value = scope.Values?.GetValueOrDefault(symbol);
        if (value is not null)
            return value;

        do
        {
            scope = scope.Parent;
            value = scope.Values?.GetValueOrDefault(symbol);
            if (value is not null)
                return value;
        }
        while (scope != scope.Parent);

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public IEnumerator<PrimValue> GetEnumerator()
    {
        foreach (var value in EnumerateValues(this))
            yield return value;

        static IEnumerable<PrimValue> EnumerateValues(EvaluatedScope? scope)
        {
            if (scope is null) yield break;
            if (scope.Values is not null)
            {
                foreach (var (_, value) in scope.Values)
                    yield return value;
            }
            foreach (var value in EnumerateValues(scope.Parent))
                yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

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

        Any = (StructValue)values[PredefinedSymbols.Any];
        Unknown = (StructValue)values[PredefinedSymbols.Unknown];
        Never = (StructValue)values[PredefinedSymbols.Never];
        Unit = (StructValue)values[PredefinedSymbols.Unit];
        Type = (StructValue)values[PredefinedSymbols.Type];
        Str = (StructValue)values[PredefinedSymbols.Str];
        Bool = (StructValue)values[PredefinedSymbols.Bool];
        I8 = (StructValue)values[PredefinedSymbols.I8];
        I16 = (StructValue)values[PredefinedSymbols.I16];
        I32 = (StructValue)values[PredefinedSymbols.I32];
        I64 = (StructValue)values[PredefinedSymbols.I64];
        I128 = (StructValue)values[PredefinedSymbols.I128];
        ISize = (StructValue)values[PredefinedSymbols.ISize];
        U8 = (StructValue)values[PredefinedSymbols.U8];
        U16 = (StructValue)values[PredefinedSymbols.U16];
        U32 = (StructValue)values[PredefinedSymbols.U32];
        U64 = (StructValue)values[PredefinedSymbols.U64];
        U128 = (StructValue)values[PredefinedSymbols.U128];
        USize = (StructValue)values[PredefinedSymbols.USize];
        F16 = (StructValue)values[PredefinedSymbols.F16];
        F32 = (StructValue)values[PredefinedSymbols.F32];
        F64 = (StructValue)values[PredefinedSymbols.F64];
        F80 = (StructValue)values[PredefinedSymbols.F80];
        F128 = (StructValue)values[PredefinedSymbols.F128];

        values[PredefinedSymbols.Print] = new LambdaValue(PredefinedSymbols.Print.LambdaType, (PrimValue x) => { Console.WriteLine(x.Value); return PrimValue.Unit; });
        values[PredefinedSymbols.Scan] = new LambdaValue(PredefinedSymbols.Scan.LambdaType, () => new LiteralValue(Str, PredefinedSymbols.Str, Console.ReadLine() ?? ""));

        Values = values;

        static Dictionary<Symbol, PrimValue> MapPredefinedTypes()
        {
            var g = GlobalBoundScope.Instance;
            var m = new Dictionary<Symbol, PrimValue>();
            foreach (var name in PredefinedSymbolNames.All)
            {
                var symbol = g.Lookup(name) as TypeSymbol
                    ?? throw new UnreachableException(DiagnosticMessage.UndefinedType(name));
                m[symbol] = new StructValue(symbol);
                // TODO: Create members for struct.
            }
            ((StructValue)m[g.Str])
                .AddEqualityOperators<string>()
                .AddMembers(s =>
                {
                    var add = g.Str.GetBinaryOperators(SyntaxKind.PlusToken, g.Str, g.Str, g.Str).Single();
                    s.Set(
                        add,
                        new LambdaValue(add.LambdaType, (PrimValue a, PrimValue b) =>
                            new LiteralValue(s, a.Type, (string)a.Value + (string)b.Value)));
                    var addStr = g.Str.GetBinaryOperators(SyntaxKind.PlusToken, g.Str, PredefinedSymbols.Any, g.Str).Single();
                    s.Set(
                        addStr,
                        new LambdaValue(addStr.LambdaType, (PrimValue a, PrimValue b) =>
                            new LiteralValue(s, a.Type, (string)a.Value + b.Value)));
                    var addAny = g.Str.GetBinaryOperators(SyntaxKind.PlusToken, PredefinedSymbols.Any, g.Str, g.Str).Single();
                    s.Set(
                        addAny,
                        new LambdaValue(addAny.LambdaType, (PrimValue a, PrimValue b) =>
                            new LiteralValue(s, a.Type, a.Value + (string)b.Value)));
                });

            R M<T>(TypeSymbol s) => new((StructValue)m[s], typeof(T));

            ((StructValue)m[g.Bool])
                .AddEqualityOperators<bool>()
                .AddLogicalOperators();
            ((StructValue)m[g.I8])
                .AddEqualityOperators<sbyte>()
                .AddComparisonOperators<sbyte>()
                .AddBitwiseOperators<sbyte>()
                .AddMathOperators<sbyte>()
                .AddImplicitConversion<sbyte>(M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<sbyte>(M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.I16])
                .AddEqualityOperators<short>()
                .AddComparisonOperators<short>()
                .AddBitwiseOperators<short>()
                .AddMathOperators<short>()
                .AddImplicitConversion<short>(M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<short>(M<sbyte>(g.I8), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.I32])
                .AddEqualityOperators<int>()
                .AddComparisonOperators<int>()
                .AddBitwiseOperators<int>()
                .AddMathOperators<int>()
                .AddImplicitConversion<int>(M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<int>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.I64])
                .AddEqualityOperators<long>()
                .AddComparisonOperators<long>()
                .AddBitwiseOperators<long>()
                .AddMathOperators<long>()
                .AddImplicitConversion<long>(M<BigInteger>(g.I128), M<nint>(g.ISize), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<long>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.I128])
                .AddEqualityOperators<BigInteger>()
                .AddComparisonOperators<BigInteger>()
                .AddBitwiseOperators<BigInteger>()
                .AddMathOperators<BigInteger>()
                .AddExplicitConversion<BigInteger>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.ISize])
                .AddEqualityOperators<nint>()
                .AddComparisonOperators<nint>()
                .AddBitwiseOperators<nint>()
                .AddMathOperators<nint>()
                .AddExplicitConversion<nint>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize))
                .AddExplicitConversion<nint>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128));
            ((StructValue)m[g.U8])
                .AddEqualityOperators<byte>()
                .AddComparisonOperators<byte>()
                .AddBitwiseOperators<byte>()
                .AddMathOperators<byte>()
                .AddImplicitConversion<byte>(M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize), M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<byte>(M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize));
            ((StructValue)m[g.U16])
                .AddEqualityOperators<ushort>()
                .AddComparisonOperators<ushort>()
                .AddBitwiseOperators<ushort>()
                .AddMathOperators<ushort>()
                .AddImplicitConversion<ushort>(M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize), M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<ushort>(M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8));
            ((StructValue)m[g.U32])
                .AddEqualityOperators<uint>()
                .AddComparisonOperators<uint>()
                .AddBitwiseOperators<uint>()
                .AddMathOperators<uint>()
                .AddImplicitConversion<uint>(M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<uint>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16));
            ((StructValue)m[g.U64])
                .AddEqualityOperators<ulong>()
                .AddComparisonOperators<ulong>()
                .AddBitwiseOperators<ulong>()
                .AddMathOperators<ulong>()
                .AddImplicitConversion<ulong>(M<BigInteger>(g.U128), M<nuint>(g.USize), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<ulong>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32));
            ((StructValue)m[g.U128])
                .AddEqualityOperators<BigInteger>()
                .AddComparisonOperators<BigInteger>()
                .AddBitwiseOperators<BigInteger>()
                .AddMathOperators<BigInteger>()
                .AddExplicitConversion<BigInteger>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64));
            ((StructValue)m[g.USize])
                .AddEqualityOperators<nuint>()
                .AddComparisonOperators<nuint>()
                .AddBitwiseOperators<nuint>()
                .AddMathOperators<nuint>()
                .AddExplicitConversion<nuint>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.F16])
                .AddEqualityOperators<Half>()
                .AddComparisonOperators<Half>()
                .AddMathOperators<Half>()
                .AddImplicitConversion<Half>(M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<Half>(M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize));
            ((StructValue)m[g.F32])
                .AddEqualityOperators<float>()
                .AddComparisonOperators<float>()
                .AddMathOperators<float>()
                .AddImplicitConversion<float>(M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<float>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize));
            ((StructValue)m[g.F64])
                .AddEqualityOperators<double>()
                .AddComparisonOperators<double>()
                .AddMathOperators<double>()
                .AddImplicitConversion<double>(M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<double>(M<Half>(g.F16), M<float>(g.F32), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize));
            // TODO: Fix these types using double.
            ((StructValue)m[g.F80])
                .AddEqualityOperators<double>()
                .AddComparisonOperators<double>()
                .AddMathOperators<double>()
                .AddImplicitConversion<double>(M<double>(g.F128))
                .AddExplicitConversion<double>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize));
            ((StructValue)m[g.F128])
                .AddEqualityOperators<double>()
                .AddComparisonOperators<double>()
                .AddMathOperators<double>()
                .AddExplicitConversion<double>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize));
            return m;
        }
    }

    public StructValue Any { get; }
    public StructValue Unknown { get; }
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
        var unaryPlus = s.TypeSymbol.GetUnaryOperators(SyntaxKind.PlusToken, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            unaryPlus,
            new LambdaValue(unaryPlus.LambdaType, (PrimValue a) => new LiteralValue(s, a.Type, +(T)a.Value)));
        var unaryMinus = s.TypeSymbol.GetUnaryOperators(SyntaxKind.MinusToken, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            unaryMinus,
            new LambdaValue(unaryMinus.LambdaType, (PrimValue a) => new LiteralValue(s, a.Type, -(T)a.Value)));
        var add = s.TypeSymbol.GetBinaryOperators(SyntaxKind.PlusToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            add,
            new LambdaValue(add.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value + (T)b.Value)));
        var subtract = s.TypeSymbol.GetBinaryOperators(SyntaxKind.MinusToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            subtract,
            new LambdaValue(subtract.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value - (T)b.Value)));
        var multiply = s.TypeSymbol.GetBinaryOperators(SyntaxKind.StarToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            multiply,
            new LambdaValue(multiply.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value * (T)b.Value)));
        var divide = s.TypeSymbol.GetBinaryOperators(SyntaxKind.SlashToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            divide,
            new LambdaValue(divide.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value / (T)b.Value)));
        var modulo = s.TypeSymbol.GetBinaryOperators(SyntaxKind.PercentToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            modulo,
            new LambdaValue(modulo.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value % (T)b.Value)));
        var power = s.TypeSymbol.GetBinaryOperators(SyntaxKind.StarStarToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            power,
            new LambdaValue(power.LambdaType, (PrimValue a, PrimValue b) => T.CreateTruncating(Math.Pow(double.CreateTruncating((T)a.Value), double.CreateTruncating((T)b.Value)))));
        return s;
    }

    public static StructValue AddBitwiseOperators<T>(this StructValue s) where T : IBinaryInteger<T>, IShiftOperators<T, int, T>
    {
        var onesComplement = s.TypeSymbol.GetUnaryOperators(SyntaxKind.TildeToken, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            onesComplement,
            new LambdaValue(onesComplement.LambdaType, (PrimValue a) => new LiteralValue(s, a.Type, ~(T)a.Value)));
        var bitwiseAnd = s.TypeSymbol.GetBinaryOperators(SyntaxKind.AmpersandToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            bitwiseAnd,
            new LambdaValue(bitwiseAnd.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value & (T)b.Value)));
        var bitwiseOr = s.TypeSymbol.GetBinaryOperators(SyntaxKind.PipeToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            bitwiseOr,
            new LambdaValue(bitwiseOr.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value | (T)b.Value)));
        var exclusiveOr = s.TypeSymbol.GetBinaryOperators(SyntaxKind.HatToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            exclusiveOr,
            new LambdaValue(exclusiveOr.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value ^ (T)b.Value)));
        var leftShift = s.TypeSymbol.GetBinaryOperators(SyntaxKind.LessThanLessThanToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            leftShift,
            new LambdaValue(leftShift.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value << int.CreateTruncating((T)b.Value))));
        var rightShift = s.TypeSymbol.GetBinaryOperators(SyntaxKind.GreaterThanGreaterThanToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            rightShift,
            new LambdaValue(rightShift.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value >> int.CreateTruncating((T)b.Value))));
        return s;
    }

    public static StructValue AddEqualityOperators<T>(this StructValue s)
    {
        var equals = s.TypeSymbol.GetBinaryOperators(SyntaxKind.EqualsEqualsToken, s.TypeSymbol, s.TypeSymbol, PredefinedSymbols.Bool).Single();
        s.Set(
            equals,
            new LambdaValue(equals.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedSymbols.Bool, ((T)a.Value).Equals(b.Value))));
        var notEquals = s.TypeSymbol.GetBinaryOperators(SyntaxKind.BangEqualsToken, s.TypeSymbol, s.TypeSymbol, PredefinedSymbols.Bool).Single();
        s.Set(
            notEquals,
            new LambdaValue(notEquals.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedSymbols.Bool, !((T)a.Value).Equals(b.Value))));
        return s;
    }

    public static StructValue AddComparisonOperators<T>(this StructValue s) where T : IComparisonOperators<T, T, bool>
    {
        var lessThan = s.TypeSymbol.GetBinaryOperators(SyntaxKind.LessThanToken, s.TypeSymbol, s.TypeSymbol, PredefinedSymbols.Bool).Single();
        s.Set(
            lessThan,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedSymbols.Bool, (T)a.Value < (T)b.Value)));
        var lessThanOrEqual = s.TypeSymbol.GetBinaryOperators(SyntaxKind.LessThanEqualsToken, s.TypeSymbol, s.TypeSymbol, PredefinedSymbols.Bool).Single();
        s.Set(
            lessThanOrEqual,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedSymbols.Bool, (T)a.Value <= (T)b.Value)));
        var greaterThan = s.TypeSymbol.GetBinaryOperators(SyntaxKind.GreaterThanToken, s.TypeSymbol, s.TypeSymbol, PredefinedSymbols.Bool).Single();
        s.Set(
            greaterThan,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedSymbols.Bool, (T)a.Value > (T)b.Value)));
        var greaterThanOrEqual = s.TypeSymbol.GetBinaryOperators(SyntaxKind.GreaterThanEqualsToken, s.TypeSymbol, s.TypeSymbol, PredefinedSymbols.Bool).Single();
        s.Set(
            greaterThanOrEqual,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedSymbols.Bool, (T)a.Value >= (T)b.Value)));
        return s;
    }

    public static StructValue AddLogicalOperators(this StructValue s)
    {
        var not = s.TypeSymbol.GetUnaryOperators(SyntaxKind.BangToken, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            not,
            new LambdaValue(not.LambdaType, (PrimValue a) =>
                new LiteralValue(s, PredefinedSymbols.Bool, !(bool)a.Value)));
        var logicalAnd = s.TypeSymbol.GetBinaryOperators(SyntaxKind.AmpersandAmpersandToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            logicalAnd,
            new LambdaValue(logicalAnd.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedSymbols.Bool, (bool)a.Value && (bool)b.Value)));
        var logicalOr = s.TypeSymbol.GetBinaryOperators(SyntaxKind.PipePipeToken, s.TypeSymbol, s.TypeSymbol, s.TypeSymbol).Single();
        s.Set(
            logicalOr,
            new LambdaValue(logicalOr.LambdaType, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedSymbols.Bool, (bool)a.Value || (bool)b.Value)));
        return s;
    }

    public static StructValue AddImplicitConversion<T>(this StructValue s, params ReadOnlySpan<R> targetTypes)
    {
        foreach (var (targetStruct, targetTypeCLR) in targetTypes)
        {
            var conversion = s.TypeSymbol.GetConversion(s.TypeSymbol, targetStruct.TypeSymbol)
                ?? throw new UnreachableException($"Missing conversion from {s.TypeSymbol} to {targetStruct.TypeSymbol}");
            s.Set(
                conversion,
                new LambdaValue(conversion.LambdaType, (PrimValue x) =>
                    new LiteralValue(targetStruct, targetStruct.TypeSymbol, Convert.ChangeType(x.Value, targetTypeCLR))));
        }
        return s;
    }

    public static StructValue AddExplicitConversion<T>(this StructValue s, params ReadOnlySpan<R> targetTypes)
    {
        foreach (var (targetStruct, targetTypeCLR) in targetTypes)
        {
            var conversion = s.TypeSymbol.GetConversion(s.TypeSymbol, targetStruct.TypeSymbol)
                ?? throw new UnreachableException($"Missing conversion from {s.TypeSymbol} to {targetStruct.TypeSymbol}");
            s.Set(
                conversion,
                new LambdaValue(conversion.LambdaType, (PrimValue x) =>
                    new LiteralValue(targetStruct, targetStruct.TypeSymbol, Convert.ChangeType(x.Value, targetTypeCLR))));
        }
        return s;
    }
}
