using System.Collections;
using System.Diagnostics;
using System.Numerics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Interpretation;

internal sealed class EvaluatedScope : IEnumerable<PrimValue>
{
    private readonly IBoundScope _boundScope;
    private Dictionary<Symbol, PrimValue>? _values;

    public EvaluatedScope(IBoundScope boundScope)
    {
        _boundScope = boundScope;
        _values = CreatePredefinedTypes(boundScope);
        Parent = this;
    }

    public EvaluatedScope(EvaluatedScope parent)
    {
        _boundScope = parent._boundScope;
        _values = null;
        Parent = parent;
    }

    public EvaluatedScope Parent { get; }

    public StructValue RuntimeType => (StructValue)Lookup(_boundScope.RuntimeType);
    public StructValue Any => (StructValue)Lookup(_boundScope.Any);
    public StructValue Err => (StructValue)Lookup(_boundScope.Err);
    public StructValue Unknown => (StructValue)Lookup(_boundScope.Unknown);
    public StructValue Never => (StructValue)Lookup(_boundScope.Never);
    public StructValue Unit => (StructValue)Lookup(_boundScope.Unit);
    public StructValue Str => (StructValue)Lookup(_boundScope.Str);
    public StructValue Bool => (StructValue)Lookup(_boundScope.Bool);
    public StructValue I8 => (StructValue)Lookup(_boundScope.I8);
    public StructValue I16 => (StructValue)Lookup(_boundScope.I16);
    public StructValue I32 => (StructValue)Lookup(_boundScope.I32);
    public StructValue I64 => (StructValue)Lookup(_boundScope.I64);
    public StructValue I128 => (StructValue)Lookup(_boundScope.I128);
    public StructValue Isz => (StructValue)Lookup(_boundScope.Isz);
    public StructValue U8 => (StructValue)Lookup(_boundScope.U8);
    public StructValue U16 => (StructValue)Lookup(_boundScope.U16);
    public StructValue U32 => (StructValue)Lookup(_boundScope.U32);
    public StructValue U64 => (StructValue)Lookup(_boundScope.U64);
    public StructValue U128 => (StructValue)Lookup(_boundScope.U128);
    public StructValue Usz => (StructValue)Lookup(_boundScope.Usz);
    public StructValue F16 => (StructValue)Lookup(_boundScope.F16);
    public StructValue F32 => (StructValue)Lookup(_boundScope.F32);
    public StructValue F64 => (StructValue)Lookup(_boundScope.F64);
    public StructValue F80 => (StructValue)Lookup(_boundScope.F80);
    public StructValue F128 => (StructValue)Lookup(_boundScope.F128);

    public void Declare(Symbol symbol, PrimValue value)
    {
        if (!(_values ??= []).TryAdd(symbol, value))
            throw new UnreachableException(DiagnosticMessage.SymbolRedeclaration(symbol.Name));
    }

    public PrimValue Lookup(Symbol symbol)
    {
        if (_values?.TryGetValue(symbol, out var value) is true)
        {
            return value;
        }

        if (!ReferenceEquals(this, Parent))
        {
            return Parent.Lookup(symbol);
        }

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public void Replace(Symbol symbol, PrimValue value)
    {
        if (_values?.ContainsKey(symbol) is true)
        {
            _values[symbol] = value;
            return;
        }

        if (!ReferenceEquals(this, Parent))
        {
            Parent.Replace(symbol, value);
        }

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public IEnumerator<PrimValue> GetEnumerator()
    {
        foreach (var value in EnumerateValues(this))
            yield return value;

        static IEnumerable<PrimValue> EnumerateValues(EvaluatedScope? scope)
        {
            if (scope is null) yield break;
            if (scope._values is not null)
            {
                foreach (var (_, value) in scope._values)
                    yield return value;
            }
            foreach (var value in EnumerateValues(scope.Parent))
                yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static Dictionary<Symbol, PrimValue> CreatePredefinedTypes(IBoundScope scope)
    {
        var m = new Dictionary<Symbol, PrimValue>();

        ReadOnlySpan<StructTypeSymbol> all = [
            scope.RuntimeType,
            scope.Any,
            scope.Err,
            scope.Unknown,
            scope.Never,
            scope.Unit,
            scope.Str,
            scope.Bool,
            scope.I8,
            scope.I16,
            scope.I32,
            scope.I64,
            scope.I128,
            scope.Isz,
            scope.U8,
            scope.U16,
            scope.U32,
            scope.U64,
            scope.U128,
            scope.Usz,
            scope.F16,
            scope.F32,
            scope.F64,
            scope.F80,
            scope.F128,
        ];

        foreach (var symbol in all)
        {
            m[symbol] = new StructValue(symbol, scope.RuntimeType);
            // TODO: Create members for struct.
        }

        var boolStruct = (StructValue)m[scope.Bool];

        ((StructValue)m[scope.Err])
            .Add(
                scope.Err.GetProperty("msg") ?? throw new UnreachableException($"Expected property '{"msg"}'"),
                new InstanceValue(((StructValue)m[scope.Str]), ""));

        ((StructValue)m[scope.Str])
            .AddEqualityOperators<string>(boolStruct)
            .AddMembers(s =>
            {
                var add = scope.Str.GetBinaryOperators(SyntaxKind.PlusToken, scope.Str, scope.Str, scope.Str).Single();
                s.Add(
                    add,
                    new LambdaValue(add.LambdaType, (PrimValue a, PrimValue b) =>
                        new InstanceValue(s, (string)a.Value + (string)b.Value)));
                var addStr = scope.Str.GetBinaryOperators(SyntaxKind.PlusToken, scope.Str, scope.Any, scope.Str).Single();
                s.Add(
                    addStr,
                    new LambdaValue(addStr.LambdaType, (PrimValue a, PrimValue b) =>
                        new InstanceValue(s, (string)a.Value + b.Value)));
                var addAny = scope.Str.GetBinaryOperators(SyntaxKind.PlusToken, scope.Any, scope.Str, scope.Str).Single();
                s.Add(
                    addAny,
                    new LambdaValue(addAny.LambdaType, (PrimValue a, PrimValue b) =>
                        new InstanceValue(s, a.Value + (string)b.Value)));
            });

        R M<T>(TypeSymbol s) => new((StructValue)m[s], typeof(T));

        ((StructValue)m[scope.Bool])
            .AddEqualityOperators<bool>(boolStruct)
            .AddLogicalOperators(boolStruct);
        ((StructValue)m[scope.I8])
            .AddEqualityOperators<sbyte>(boolStruct)
            .AddComparisonOperators<sbyte>(boolStruct)
            .AddBitwiseOperators<sbyte>()
            .AddMathOperators<sbyte>()
            .AddImplicitConversion<sbyte>(M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<Half>(scope.F16), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<sbyte>(M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128));
        ((StructValue)m[scope.I16])
            .AddEqualityOperators<short>(boolStruct)
            .AddComparisonOperators<short>(boolStruct)
            .AddBitwiseOperators<short>()
            .AddMathOperators<short>()
            .AddImplicitConversion<short>(M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<Half>(scope.F16), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<short>(M<sbyte>(scope.I8), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128));
        ((StructValue)m[scope.I32])
            .AddEqualityOperators<int>(boolStruct)
            .AddComparisonOperators<int>(boolStruct)
            .AddBitwiseOperators<int>()
            .AddMathOperators<int>()
            .AddImplicitConversion<int>(M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<int>(M<Half>(scope.F16), M<sbyte>(scope.I8), M<short>(scope.I16), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128));
        ((StructValue)m[scope.I64])
            .AddEqualityOperators<long>(boolStruct)
            .AddComparisonOperators<long>(boolStruct)
            .AddBitwiseOperators<long>()
            .AddMathOperators<long>()
            .AddImplicitConversion<long>(M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<long>(M<Half>(scope.F16), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128));
        ((StructValue)m[scope.I128])
            .AddEqualityOperators<BigInteger>(boolStruct)
            .AddComparisonOperators<BigInteger>(boolStruct)
            .AddBitwiseOperators<BigInteger>()
            .AddMathOperators<BigInteger>()
            .AddExplicitConversion<BigInteger>(M<Half>(scope.F16), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<nint>(scope.Isz), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128));
        ((StructValue)m[scope.Isz])
            .AddEqualityOperators<nint>(boolStruct)
            .AddComparisonOperators<nint>(boolStruct)
            .AddBitwiseOperators<nint>()
            .AddMathOperators<nint>()
            .AddExplicitConversion<nint>(M<Half>(scope.F16), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128), M<nuint>(scope.Usz));
        ((StructValue)m[scope.U8])
            .AddEqualityOperators<byte>(boolStruct)
            .AddComparisonOperators<byte>(boolStruct)
            .AddBitwiseOperators<byte>()
            .AddMathOperators<byte>()
            .AddImplicitConversion<byte>(M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128), M<nuint>(scope.Usz), M<Half>(scope.F16), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<byte>(M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz));
        ((StructValue)m[scope.U16])
            .AddEqualityOperators<ushort>(boolStruct)
            .AddComparisonOperators<ushort>(boolStruct)
            .AddBitwiseOperators<ushort>()
            .AddMathOperators<ushort>()
            .AddImplicitConversion<ushort>(M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128), M<nuint>(scope.Usz), M<Half>(scope.F16), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<ushort>(M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<byte>(scope.U8));
        ((StructValue)m[scope.U32])
            .AddEqualityOperators<uint>(boolStruct)
            .AddComparisonOperators<uint>(boolStruct)
            .AddBitwiseOperators<uint>()
            .AddMathOperators<uint>()
            .AddImplicitConversion<uint>(M<ulong>(scope.U64), M<BigInteger>(scope.U128), M<nuint>(scope.Usz), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<uint>(M<Half>(scope.F16), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<byte>(scope.U8), M<ushort>(scope.U16));
        ((StructValue)m[scope.U64])
            .AddEqualityOperators<ulong>(boolStruct)
            .AddComparisonOperators<ulong>(boolStruct)
            .AddBitwiseOperators<ulong>()
            .AddMathOperators<ulong>()
            .AddImplicitConversion<ulong>(M<BigInteger>(scope.U128), M<nuint>(scope.Usz), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<ulong>(M<Half>(scope.F16), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32));
        ((StructValue)m[scope.U128])
            .AddEqualityOperators<BigInteger>(boolStruct)
            .AddComparisonOperators<BigInteger>(boolStruct)
            .AddBitwiseOperators<BigInteger>()
            .AddMathOperators<BigInteger>()
            .AddExplicitConversion<BigInteger>(M<Half>(scope.F16), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64));
        ((StructValue)m[scope.Usz])
            .AddEqualityOperators<nuint>(boolStruct)
            .AddComparisonOperators<nuint>(boolStruct)
            .AddBitwiseOperators<nuint>()
            .AddMathOperators<nuint>()
            .AddExplicitConversion<nuint>(M<Half>(scope.F16), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128));
        ((StructValue)m[scope.F16])
            .AddEqualityOperators<Half>(boolStruct)
            .AddComparisonOperators<Half>(boolStruct)
            .AddMathOperators<Half>()
            .AddImplicitConversion<Half>(M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<Half>(M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128), M<nuint>(scope.Usz));
        ((StructValue)m[scope.F32])
            .AddEqualityOperators<float>(boolStruct)
            .AddComparisonOperators<float>(boolStruct)
            .AddMathOperators<float>()
            .AddImplicitConversion<float>(M<double>(scope.F64), M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<float>(M<Half>(scope.F16), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128), M<nuint>(scope.Usz));
        ((StructValue)m[scope.F64])
            .AddEqualityOperators<double>(boolStruct)
            .AddComparisonOperators<double>(boolStruct)
            .AddMathOperators<double>()
            .AddImplicitConversion<double>(M<double>(scope.F80), M<double>(scope.F128))
            .AddExplicitConversion<double>(M<Half>(scope.F16), M<float>(scope.F32), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128), M<nuint>(scope.Usz));
        // TODO: Fix these types using double.
        ((StructValue)m[scope.F80])
            .AddEqualityOperators<double>(boolStruct)
            .AddComparisonOperators<double>(boolStruct)
            .AddMathOperators<double>()
            .AddImplicitConversion<double>(M<double>(scope.F128))
            .AddExplicitConversion<double>(M<Half>(scope.F16), M<float>(scope.F32), M<double>(scope.F64), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128), M<nuint>(scope.Usz));
        ((StructValue)m[scope.F128])
            .AddEqualityOperators<double>(boolStruct)
            .AddComparisonOperators<double>(boolStruct)
            .AddMathOperators<double>()
            .AddExplicitConversion<double>(M<Half>(scope.F16), M<float>(scope.F32), M<double>(scope.F64), M<double>(scope.F80), M<sbyte>(scope.I8), M<short>(scope.I16), M<int>(scope.I32), M<long>(scope.I64), M<BigInteger>(scope.I128), M<nint>(scope.Isz), M<byte>(scope.U8), M<ushort>(scope.U16), M<uint>(scope.U32), M<ulong>(scope.U64), M<BigInteger>(scope.U128), M<nuint>(scope.Usz));
        return m;
    }
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
        var equals = s.StructType.GetBinaryOperators(SyntaxKind.EqualsEqualsToken, s.StructType, s.StructType, boolStruct.StructType).Single();
        s.Add(
            equals,
            new LambdaValue(equals.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, ((T)a.Value).Equals(b.Value))));
        var notEquals = s.StructType.GetBinaryOperators(SyntaxKind.BangEqualsToken, s.StructType, s.StructType, boolStruct.StructType).Single();
        s.Add(
            notEquals,
            new LambdaValue(notEquals.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, !((T)a.Value).Equals(b.Value))));
        return s;
    }

    public static StructValue AddComparisonOperators<T>(this StructValue s, StructValue boolStruct) where T : IComparisonOperators<T, T, bool>
    {
        var lessThan = s.StructType.GetBinaryOperators(SyntaxKind.LessThanToken, s.StructType, s.StructType, boolStruct.StructType).Single();
        s.Add(
            lessThan,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, (T)a.Value < (T)b.Value)));
        var lessThanOrEqual = s.StructType.GetBinaryOperators(SyntaxKind.LessThanEqualsToken, s.StructType, s.StructType, boolStruct.StructType).Single();
        s.Add(
            lessThanOrEqual,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, (T)a.Value <= (T)b.Value)));
        var greaterThan = s.StructType.GetBinaryOperators(SyntaxKind.GreaterThanToken, s.StructType, s.StructType, boolStruct.StructType).Single();
        s.Add(
            greaterThan,
            new LambdaValue(lessThan.LambdaType, (PrimValue a, PrimValue b) =>
                new InstanceValue(boolStruct, (T)a.Value > (T)b.Value)));
        var greaterThanOrEqual = s.StructType.GetBinaryOperators(SyntaxKind.GreaterThanEqualsToken, s.StructType, s.StructType, boolStruct.StructType).Single();
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

