﻿using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    public bool TryLookup(Symbol symbol, [MaybeNullWhen(false)] out PrimValue value)
    {
        var scope = this;
        value = scope.Values?.GetValueOrDefault(symbol);
        if (value is not null)
            return true;

        do
        {
            scope = scope.Parent;
            value = scope.Values?.GetValueOrDefault(symbol);
            if (value is not null)
                return true;
        }
        while (scope != scope.Parent);

        return false;
    }

    public PrimValue Lookup(Symbol symbol)
    {
        if (!TryLookup(symbol, out var value))
            throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
        return value;
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
                var symbol = g.Lookup(name) as TypeSymbol
                    ?? throw new UnreachableException(DiagnosticMessage.UndefinedType(name));
                m[symbol] = new StructValue(symbol);
                // TODO: Create members for struct.
            }
            ((StructValue)m[g.Str])
                .AddEqualityOperators<string>()
                .AddMembers(s =>
                {
                    var add = g.Str.Type.GetBinaryOperators(SyntaxKind.PlusToken, g.Str.Type, g.Str.Type, g.Str.Type).Single();
                    s.Set(
                        MethodSymbol.FromOperator(add),
                        new FunctionValue(add.Type, (PrimValue a, PrimValue b) =>
                            new LiteralValue(s, a.Type, (string)a.Value + (string)b.Value)));
                    var addStr = g.Str.Type.GetBinaryOperators(SyntaxKind.PlusToken, g.Str.Type, PredefinedTypes.Any, g.Str.Type).Single();
                    s.Set(
                        MethodSymbol.FromOperator(addStr),
                        new FunctionValue(addStr.Type, (PrimValue a, PrimValue b) =>
                            new LiteralValue(s, a.Type, (string)a.Value + b.Value)));
                    var addAny = g.Str.Type.GetBinaryOperators(SyntaxKind.PlusToken, PredefinedTypes.Any, g.Str.Type, g.Str.Type).Single();
                    s.Set(
                        MethodSymbol.FromOperator(addAny),
                        new FunctionValue(addAny.Type, (PrimValue a, PrimValue b) =>
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
        var unaryPlus = s.TypeSymbol.Type.GetUnaryOperators(SyntaxKind.PlusToken, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(unaryPlus),
            new FunctionValue(unaryPlus.Type, (PrimValue a) => new LiteralValue(s, a.Type, +(T)a.Value)));
        var unaryMinus = s.TypeSymbol.Type.GetUnaryOperators(SyntaxKind.MinusToken, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(unaryMinus),
            new FunctionValue(unaryMinus.Type, (PrimValue a) => new LiteralValue(s, a.Type, -(T)a.Value)));
        var add = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.PlusToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(add),
            new FunctionValue(add.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value + (T)b.Value)));
        var subtract = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.MinusToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(subtract),
            new FunctionValue(subtract.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value - (T)b.Value)));
        var multiply = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.StarToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(multiply),
            new FunctionValue(multiply.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value * (T)b.Value)));
        var divide = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.SlashToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(divide),
            new FunctionValue(divide.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value / (T)b.Value)));
        var modulo = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.PercentToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(modulo),
            new FunctionValue(modulo.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value % (T)b.Value)));
        var power = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.StarStarToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(power),
            new FunctionValue(power.Type, (PrimValue a, PrimValue b) => T.CreateTruncating(Math.Pow(double.CreateTruncating((T)a.Value), double.CreateTruncating((T)b.Value)))));
        return s;
    }

    public static StructValue AddBitwiseOperators<T>(this StructValue s) where T : IBinaryInteger<T>, IShiftOperators<T, int, T>
    {
        var onesComplement = s.TypeSymbol.Type.GetUnaryOperators(SyntaxKind.TildeToken, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(onesComplement),
            new FunctionValue(onesComplement.Type, (PrimValue a) => new LiteralValue(s, a.Type, ~(T)a.Value)));
        var bitwiseAnd = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.AmpersandToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(bitwiseAnd),
            new FunctionValue(bitwiseAnd.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value & (T)b.Value)));
        var bitwiseOr = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.PipeToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(bitwiseOr),
            new FunctionValue(bitwiseOr.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value | (T)b.Value)));
        var exclusiveOr = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.HatToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(exclusiveOr),
            new FunctionValue(exclusiveOr.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value ^ (T)b.Value)));
        var leftShift = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.LessThanLessThanToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(leftShift),
            new FunctionValue(leftShift.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value << int.CreateTruncating((T)b.Value))));
        var rightShift = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.GreaterThanGreaterThanToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(rightShift),
            new FunctionValue(rightShift.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, a.Type, (T)a.Value >> int.CreateTruncating((T)b.Value))));
        return s;
    }

    public static StructValue AddEqualityOperators<T>(this StructValue s)
    {
        var equals = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.EqualsEqualsToken, s.TypeSymbol.Type, s.TypeSymbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            MethodSymbol.FromOperator(equals),
            new FunctionValue(equals.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, ((T)a.Value).Equals(b.Value))));
        var notEquals = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.BangEqualsToken, s.TypeSymbol.Type, s.TypeSymbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            MethodSymbol.FromOperator(notEquals),
            new FunctionValue(notEquals.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, !((T)a.Value).Equals(b.Value))));
        return s;
    }

    public static StructValue AddComparisonOperators<T>(this StructValue s) where T : IComparisonOperators<T, T, bool>
    {
        var lessThan = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.LessThanToken, s.TypeSymbol.Type, s.TypeSymbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            MethodSymbol.FromOperator(lessThan),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (T)a.Value < (T)b.Value)));
        var lessThanOrEqual = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.LessThanEqualsToken, s.TypeSymbol.Type, s.TypeSymbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            MethodSymbol.FromOperator(lessThanOrEqual),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (T)a.Value <= (T)b.Value)));
        var greaterThan = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.GreaterThanToken, s.TypeSymbol.Type, s.TypeSymbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            MethodSymbol.FromOperator(greaterThan),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (T)a.Value > (T)b.Value)));
        var greaterThanOrEqual = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.GreaterThanEqualsToken, s.TypeSymbol.Type, s.TypeSymbol.Type, PredefinedTypes.Bool).Single();
        s.Set(
            MethodSymbol.FromOperator(greaterThanOrEqual),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (T)a.Value >= (T)b.Value)));
        return s;
    }

    public static StructValue AddLogicalOperators(this StructValue s)
    {
        var not = s.TypeSymbol.Type.GetUnaryOperators(SyntaxKind.BangToken, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(not),
            new FunctionValue(not.Type, (PrimValue a) =>
                new LiteralValue(s, PredefinedTypes.Bool, !(bool)a.Value)));
        var logicalAnd = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.AmpersandAmpersandToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(logicalAnd),
            new FunctionValue(logicalAnd.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (bool)a.Value && (bool)b.Value)));
        var logicalOr = s.TypeSymbol.Type.GetBinaryOperators(SyntaxKind.PipePipeToken, s.TypeSymbol.Type, s.TypeSymbol.Type, s.TypeSymbol.Type).Single();
        s.Set(
            MethodSymbol.FromOperator(logicalOr),
            new FunctionValue(logicalOr.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(s, PredefinedTypes.Bool, (bool)a.Value || (bool)b.Value)));
        return s;
    }

    public static StructValue AddImplicitConversion<T>(this StructValue s, params ReadOnlySpan<R> targetTypes)
    {
        foreach (var (targetStruct, targetTypeCLR) in targetTypes)
        {
            var conversion = s.TypeSymbol.Type.GetConversion(s.TypeSymbol.Type, targetStruct.TypeSymbol.Type)
                ?? throw new UnreachableException($"Missing conversion from {s.TypeSymbol.Type} to {targetStruct.TypeSymbol.Type}");
            s.Set(
                MethodSymbol.FromConversion(conversion),
                new FunctionValue(conversion.Type, (PrimValue x) =>
                    new LiteralValue(targetStruct, targetStruct.TypeSymbol.Type, Convert.ChangeType(x.Value, targetTypeCLR))));
        }
        return s;
    }

    public static StructValue AddExplicitConversion<T>(this StructValue s, params ReadOnlySpan<R> targetTypes)
    {
        foreach (var (targetStruct, targetTypeCLR) in targetTypes)
        {
            var conversion = s.TypeSymbol.Type.GetConversion(s.TypeSymbol.Type, targetStruct.TypeSymbol.Type)
                ?? throw new UnreachableException($"Missing conversion from {s.TypeSymbol.Type} to {targetStruct.TypeSymbol.Type}");
            s.Set(
                MethodSymbol.FromConversion(conversion),
                new FunctionValue(conversion.Type, (PrimValue x) =>
                    new LiteralValue(targetStruct, targetStruct.TypeSymbol.Type, Convert.ChangeType(x.Value, targetTypeCLR))));
        }
        return s;
    }
}
