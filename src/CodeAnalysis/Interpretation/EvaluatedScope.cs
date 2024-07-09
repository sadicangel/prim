using System.Collections;
using System.Diagnostics;
using System.Numerics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;

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

        Any = (StructValue)values[GlobalBoundScope.Instance.Any];
        Unknown = (StructValue)values[GlobalBoundScope.Instance.Unknown];
        Never = (StructValue)values[GlobalBoundScope.Instance.Never];
        Unit = (StructValue)values[GlobalBoundScope.Instance.Unit];
        Type = (StructValue)values[GlobalBoundScope.Instance.Type];
        Str = (StructValue)values[GlobalBoundScope.Instance.Str];
        Bool = (StructValue)values[GlobalBoundScope.Instance.Bool];
        I8 = (StructValue)values[GlobalBoundScope.Instance.I8];
        I16 = (StructValue)values[GlobalBoundScope.Instance.I16];
        I32 = (StructValue)values[GlobalBoundScope.Instance.I32];
        I64 = (StructValue)values[GlobalBoundScope.Instance.I64];
        I128 = (StructValue)values[GlobalBoundScope.Instance.I128];
        ISize = (StructValue)values[GlobalBoundScope.Instance.ISize];
        U8 = (StructValue)values[GlobalBoundScope.Instance.U8];
        U16 = (StructValue)values[GlobalBoundScope.Instance.U16];
        U32 = (StructValue)values[GlobalBoundScope.Instance.U32];
        U64 = (StructValue)values[GlobalBoundScope.Instance.U64];
        U128 = (StructValue)values[GlobalBoundScope.Instance.U128];
        USize = (StructValue)values[GlobalBoundScope.Instance.USize];
        F16 = (StructValue)values[GlobalBoundScope.Instance.F16];
        F32 = (StructValue)values[GlobalBoundScope.Instance.F32];
        F64 = (StructValue)values[GlobalBoundScope.Instance.F64];
        F80 = (StructValue)values[GlobalBoundScope.Instance.F80];
        F128 = (StructValue)values[GlobalBoundScope.Instance.F128];

        Values = values;

        static Dictionary<Symbol, PrimValue> MapPredefinedTypes()
        {
            var g = GlobalBoundScope.Instance;
            var m = new Dictionary<Symbol, PrimValue>();
            foreach (var name in PredefinedTypeNames.All)
            {
                var symbol = g.Lookup(name) as StructSymbol
                    ?? throw new UnreachableException(DiagnosticMessage.UndefinedType(name));
                m[symbol] = new StructValue(symbol.Type);
                // TODO: Create members for struct.
            }
            ((StructValue)m[g.Str])
                .AddEqualityOperators<string>(g.Str)
                .AddMembers(s =>
                {
                    var add = g.Str.Type.GetBinaryOperators(SyntaxKind.AddOperator, g.Str.Type, g.Str.Type, g.Str.Type).Single();
                    s.Set(
                        FunctionSymbol.FromOperator(add),
                        new FunctionValue(add.Type, (PrimValue a, PrimValue b) =>
                            new LiteralValue(s, a.Type, (string)a.Value + (string)b.Value)));
                    var addStr = g.Str.Type.GetBinaryOperators(SyntaxKind.AddOperator, g.Str.Type, PredefinedTypes.Any, g.Str.Type).Single();
                    s.Set(
                        FunctionSymbol.FromOperator(addStr),
                        new FunctionValue(addStr.Type, (PrimValue a, PrimValue b) =>
                            new LiteralValue(s, a.Type, (string)a.Value + b.Value)));
                    var addAny = g.Str.Type.GetBinaryOperators(SyntaxKind.AddOperator, PredefinedTypes.Any, g.Str.Type, g.Str.Type).Single();
                    s.Set(
                        FunctionSymbol.FromOperator(addAny),
                        new FunctionValue(addAny.Type, (PrimValue a, PrimValue b) =>
                            new LiteralValue(s, a.Type, a.Value + (string)b.Value)));
                });

            R M<T>(StructSymbol s) => new((StructValue)m[s], typeof(T));

            ((StructValue)m[g.Bool])
                .AddEqualityOperators<bool>(g.Bool)
                .AddLogicalOperators(g.Bool);
            ((StructValue)m[g.I8])
                .AddEqualityOperators<sbyte>(g.I8)
                .AddComparisonOperators<sbyte>(g.I8)
                .AddBitwiseOperators<sbyte>(g.I8)
                .AddMathOperators<sbyte>(g.I8)
                .AddImplicitConversion<sbyte>(M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<sbyte>(M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.I16])
                .AddEqualityOperators<short>(g.I16)
                .AddComparisonOperators<short>(g.I16)
                .AddBitwiseOperators<short>(g.I16)
                .AddMathOperators<short>(g.I16)
                .AddImplicitConversion<short>(M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<short>(M<sbyte>(g.I8), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.I32])
                .AddEqualityOperators<int>(g.I32)
                .AddComparisonOperators<int>(g.I32)
                .AddBitwiseOperators<int>(g.I32)
                .AddMathOperators<int>(g.I32)
                .AddImplicitConversion<int>(M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<int>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.I64])
                .AddEqualityOperators<long>(g.I64)
                .AddComparisonOperators<long>(g.I64)
                .AddBitwiseOperators<long>(g.I64)
                .AddMathOperators<long>(g.I64)
                .AddImplicitConversion<long>(M<BigInteger>(g.I128), M<nint>(g.ISize), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<long>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.I128])
                .AddEqualityOperators<BigInteger>(g.I64)
                .AddComparisonOperators<BigInteger>(g.I64)
                .AddBitwiseOperators<BigInteger>(g.I64)
                .AddMathOperators<BigInteger>(g.I64)
                .AddExplicitConversion<BigInteger>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.ISize])
                .AddEqualityOperators<nint>(g.I64)
                .AddComparisonOperators<nint>(g.I64)
                .AddBitwiseOperators<nint>(g.I64)
                .AddMathOperators<nint>(g.I64)
                .AddExplicitConversion<nint>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize))
                .AddExplicitConversion<nint>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128));
            ((StructValue)m[g.U8])
                .AddEqualityOperators<byte>(g.U8)
                .AddComparisonOperators<byte>(g.U8)
                .AddBitwiseOperators<byte>(g.U8)
                .AddMathOperators<byte>(g.U8)
                .AddImplicitConversion<byte>(M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize), M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<byte>(M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize));
            ((StructValue)m[g.U16])
                .AddEqualityOperators<ushort>(g.U16)
                .AddComparisonOperators<ushort>(g.U16)
                .AddBitwiseOperators<ushort>(g.U16)
                .AddMathOperators<ushort>(g.U16)
                .AddImplicitConversion<ushort>(M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize), M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<ushort>(M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8));
            ((StructValue)m[g.U32])
                .AddEqualityOperators<uint>(g.U32)
                .AddComparisonOperators<uint>(g.U32)
                .AddBitwiseOperators<uint>(g.U32)
                .AddMathOperators<uint>(g.U32)
                .AddImplicitConversion<uint>(M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<uint>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16));
            ((StructValue)m[g.U64])
                .AddEqualityOperators<ulong>(g.U64)
                .AddComparisonOperators<ulong>(g.U64)
                .AddBitwiseOperators<ulong>(g.U64)
                .AddMathOperators<ulong>(g.U64)
                .AddImplicitConversion<ulong>(M<BigInteger>(g.U128), M<nuint>(g.USize), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<ulong>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32));
            ((StructValue)m[g.U128])
                .AddEqualityOperators<BigInteger>(g.U64)
                .AddComparisonOperators<BigInteger>(g.U64)
                .AddBitwiseOperators<BigInteger>(g.U64)
                .AddMathOperators<BigInteger>(g.U64)
                .AddExplicitConversion<BigInteger>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64));
            ((StructValue)m[g.USize])
                .AddEqualityOperators<nuint>(g.U64)
                .AddComparisonOperators<nuint>(g.U64)
                .AddBitwiseOperators<nuint>(g.U64)
                .AddMathOperators<nuint>(g.U64)
                .AddExplicitConversion<nuint>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128));
            ((StructValue)m[g.F16])
                .AddEqualityOperators<Half>(g.F16)
                .AddComparisonOperators<Half>(g.F16)
                .AddMathOperators<Half>(g.F16)
                .AddImplicitConversion<Half>(M<float>(g.F32), M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<Half>(M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize));
            ((StructValue)m[g.F32])
                .AddEqualityOperators<float>(g.F32)
                .AddComparisonOperators<float>(g.F32)
                .AddMathOperators<float>(g.F32)
                .AddImplicitConversion<float>(M<double>(g.F64), M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<float>(M<Half>(g.F16), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize));
            ((StructValue)m[g.F64])
                .AddEqualityOperators<double>(g.F64)
                .AddComparisonOperators<double>(g.F64)
                .AddMathOperators<double>(g.F64)
                .AddImplicitConversion<double>(M<double>(g.F80), M<double>(g.F128))
                .AddExplicitConversion<double>(M<Half>(g.F16), M<float>(g.F32), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize));
            // TODO: Fix these types using double.
            ((StructValue)m[g.F80])
                .AddEqualityOperators<double>(g.F80)
                .AddComparisonOperators<double>(g.F80)
                .AddMathOperators<double>(g.F80)
                .AddImplicitConversion<double>(M<double>(g.F128))
                .AddExplicitConversion<double>(M<Half>(g.F16), M<float>(g.F32), M<double>(g.F64), M<sbyte>(g.I8), M<short>(g.I16), M<int>(g.I32), M<long>(g.I64), M<BigInteger>(g.I128), M<nint>(g.ISize), M<byte>(g.U8), M<ushort>(g.U16), M<uint>(g.U32), M<ulong>(g.U64), M<BigInteger>(g.U128), M<nuint>(g.USize));
            ((StructValue)m[g.F128])
                .AddEqualityOperators<double>(g.F128)
                .AddComparisonOperators<double>(g.F128)
                .AddMathOperators<double>(g.F128)
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

    public static StructValue AddMathOperators<T>(this StructValue s, StructSymbol symbol) where T : INumber<T>
    {
        var unaryPlus = symbol.Type.GetUnaryOperators(SyntaxKind.UnaryPlusOperator, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(unaryPlus),
            new FunctionValue(unaryPlus.Type, (PrimValue a) => new LiteralValue(s, a.Type, +(T)a.Value)));
        var unaryMinus = symbol.Type.GetUnaryOperators(SyntaxKind.UnaryMinusOperator, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(unaryMinus),
            new FunctionValue(unaryMinus.Type, (PrimValue a) => new LiteralValue(s, a.Type, -(T)a.Value)));
        var add = symbol.Type.GetBinaryOperators(SyntaxKind.AddOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(add),
            new FunctionValue(add.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value + (T)b.Value)));
        var subtract = symbol.Type.GetBinaryOperators(SyntaxKind.SubtractOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(subtract),
            new FunctionValue(subtract.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value - (T)b.Value)));
        var multiply = symbol.Type.GetBinaryOperators(SyntaxKind.MultiplyOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(multiply),
            new FunctionValue(multiply.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value * (T)b.Value)));
        var divide = symbol.Type.GetBinaryOperators(SyntaxKind.DivideOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(divide),
            new FunctionValue(divide.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value / (T)b.Value)));
        var modulo = symbol.Type.GetBinaryOperators(SyntaxKind.ModuloOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(modulo),
            new FunctionValue(modulo.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value % (T)b.Value)));
        var power = symbol.Type.GetBinaryOperators(SyntaxKind.PowerOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(power),
            new FunctionValue(power.Type, (PrimValue a, PrimValue b) => T.CreateTruncating(Math.Pow(double.CreateTruncating((T)a.Value), double.CreateTruncating((T)b.Value)))));
        return s;
    }

    public static StructValue AddBitwiseOperators<T>(this StructValue s, StructSymbol symbol) where T : IBinaryInteger<T>, IShiftOperators<T, int, T>
    {
        var onesComplement = symbol.Type.GetUnaryOperators(SyntaxKind.OnesComplementOperator, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(onesComplement),
            new FunctionValue(onesComplement.Type, (PrimValue a) => new LiteralValue(s, a.Type, ~(T)a.Value)));
        var bitwiseAnd = symbol.Type.GetBinaryOperators(SyntaxKind.BitwiseAndOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(bitwiseAnd),
            new FunctionValue(bitwiseAnd.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value & (T)b.Value)));
        var bitwiseOr = symbol.Type.GetBinaryOperators(SyntaxKind.BitwiseOrOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(bitwiseOr),
            new FunctionValue(bitwiseOr.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value | (T)b.Value)));
        var exclusiveOr = symbol.Type.GetBinaryOperators(SyntaxKind.ExclusiveOrOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(exclusiveOr),
            new FunctionValue(exclusiveOr.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value ^ (T)b.Value)));
        var leftShift = symbol.Type.GetBinaryOperators(SyntaxKind.LeftShiftOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(leftShift),
            new FunctionValue(leftShift.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value << int.CreateTruncating((T)b.Value))));
        var rightShift = symbol.Type.GetBinaryOperators(SyntaxKind.RightShiftOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(rightShift),
            new FunctionValue(rightShift.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value >> int.CreateTruncating((T)b.Value))));
        return s;
    }

    public static StructValue AddEqualityOperators<T>(this StructValue s, StructSymbol symbol)
    {
        var equals = symbol.Type.GetBinaryOperators(SyntaxKind.EqualsOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            FunctionSymbol.FromOperator(equals),
            new FunctionValue(equals.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, ((T)a.Value).Equals(b.Value))));
        var notEquals = symbol.Type.GetBinaryOperators(SyntaxKind.NotEqualsOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            FunctionSymbol.FromOperator(notEquals),
            new FunctionValue(notEquals.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, !((T)a.Value).Equals(b.Value))));
        return s;
    }

    public static StructValue AddComparisonOperators<T>(this StructValue s, StructSymbol symbol) where T : IComparisonOperators<T, T, bool>
    {
        var lessThan = symbol.Type.GetBinaryOperators(SyntaxKind.LessThanOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            FunctionSymbol.FromOperator(lessThan),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (T)a.Value < (T)b.Value)));
        var lessThanOrEqual = symbol.Type.GetBinaryOperators(SyntaxKind.LessThanOrEqualOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            FunctionSymbol.FromOperator(lessThanOrEqual),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (T)a.Value <= (T)b.Value)));
        var greaterThan = symbol.Type.GetBinaryOperators(SyntaxKind.GreaterThanOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            FunctionSymbol.FromOperator(greaterThan),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (T)a.Value > (T)b.Value)));
        var greaterThanOrEqual = symbol.Type.GetBinaryOperators(SyntaxKind.GreaterThanOrEqualOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            FunctionSymbol.FromOperator(greaterThanOrEqual),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (T)a.Value >= (T)b.Value)));
        return s;
    }

    public static StructValue AddLogicalOperators(this StructValue s, StructSymbol symbol)
    {
        var not = symbol.Type.GetUnaryOperators(SyntaxKind.NotOperator, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(not),
            new FunctionValue(not.Type, (PrimValue a) =>
                new LiteralValue(s, PredefinedTypes.Bool, !(bool)a.Value)));
        var logicalAnd = symbol.Type.GetBinaryOperators(SyntaxKind.LogicalAndOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(logicalAnd),
            new FunctionValue(logicalAnd.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (bool)a.Value && (bool)b.Value)));
        var logicalOr = symbol.Type.GetBinaryOperators(SyntaxKind.LogicalOrOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.Set(
            FunctionSymbol.FromOperator(logicalOr),
            new FunctionValue(logicalOr.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (bool)a.Value || (bool)b.Value)));
        return s;
    }

    public static StructValue AddImplicitConversion<T>(this StructValue s, params ReadOnlySpan<R> targetTypes)
    {
        foreach (var (targetStruct, targetTypeCLR) in targetTypes)
        {
            var conversion = s.StructType.GetConversion(s.StructType, targetStruct.StructType)
                ?? throw new UnreachableException($"Missing conversion from {s.StructType} to {targetStruct.StructType}");
            s.Set(
                FunctionSymbol.FromConversion(conversion),
                new FunctionValue(conversion.Type, (PrimValue x) =>
                    new LiteralValue(targetStruct, targetStruct.StructType, Convert.ChangeType(x.Value, targetTypeCLR))));
        }
        return s;
    }

    public static StructValue AddExplicitConversion<T>(this StructValue s, params ReadOnlySpan<R> targetTypes)
    {
        foreach (var (targetStruct, targetTypeCLR) in targetTypes)
        {
            var conversion = s.StructType.GetConversion(s.StructType, targetStruct.StructType)
                ?? throw new UnreachableException($"Missing conversion from {s.StructType} to {targetStruct.StructType}");
            s.Set(
                FunctionSymbol.FromConversion(conversion),
                new FunctionValue(conversion.Type, (PrimValue x) =>
                    new LiteralValue(targetStruct, targetStruct.StructType, Convert.ChangeType(x.Value, targetTypeCLR))));
        }
        return s;
    }
}
