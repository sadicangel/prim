using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CodeAnalysis.Symbols;

internal static class BuiltinTypes
{
    private static readonly Lazy<ConcurrentDictionary<string, TypeSymbol>> TypeMap = new(() => new ConcurrentDictionary<string, TypeSymbol>(typeof(BuiltinTypes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(TypeSymbol))
        .Select(f => (TypeSymbol)f.GetValue(null)!)
        .ToDictionary(f => f.Name)));

    public static readonly TypeSymbol Never = new("never")
    {
        ClrType = typeof(void)
    };

    public static readonly TypeSymbol Any = new("any")
    {
        ClrType = typeof(object),
    };

    public static readonly TypeSymbol Void = new("void")
    {
        ClrType = typeof(void),
    };

    public static readonly TypeSymbol Type = new("type")
    {
        ClrType = typeof(Type),
    };

    public static readonly TypeSymbol Bool = new("bool")
    {
        ClrType = typeof(bool),
    };

    public static readonly TypeSymbol I8 = new("i8")
    {
        ClrType = typeof(sbyte),
    };
    public static readonly TypeSymbol I16 = new("i16")
    {
        ClrType = typeof(short),
    };
    public static readonly TypeSymbol I32 = new("i32")
    {
        ClrType = typeof(int),
    };
    public static readonly TypeSymbol I64 = new("i64")
    {
        ClrType = typeof(long),
    };
    public static readonly TypeSymbol ISize = new("isize")
    {
        ClrType = typeof(nint),
    };

    public static readonly TypeSymbol U8 = new("u8")
    {
        ClrType = typeof(byte),
    };
    public static readonly TypeSymbol U16 = new("u16")
    {
        ClrType = typeof(ushort),
    };
    public static readonly TypeSymbol U32 = new("u32")
    {
        ClrType = typeof(uint),
    };
    public static readonly TypeSymbol U64 = new("u64")
    {
        ClrType = typeof(ulong),
    };
    public static readonly TypeSymbol USize = new("usize")
    {
        ClrType = typeof(nuint),
    };

    public static readonly TypeSymbol F32 = new("f32")
    {
        ClrType = typeof(float),
    };
    public static readonly TypeSymbol F64 = new("f64")
    {
        ClrType = typeof(double),
    };

    public static readonly TypeSymbol Str = new("str")
    {
        ClrType = typeof(string),
    };

    public static readonly TypeSymbol Func = new("func")
    {
        ClrType = typeof(Delegate),
    };

    public static IEnumerable<TypeSymbol> All { get => TypeMap.Value.Values; }

    public static bool TryLookup(string name, [MaybeNullWhen(false)] out TypeSymbol type) => TypeMap.Value.TryGetValue(name, out type);

    public static int SizeOf(this TypeSymbol type) => type.Name switch
    {
        "bool" => sizeof(bool),
        "i8" => sizeof(sbyte),
        "i16" => sizeof(short),
        "i32" => sizeof(int),
        "i64" => sizeof(long),
        "isize" => Unsafe.SizeOf<nint>(),
        "u8" => sizeof(byte),
        "u16" => sizeof(ushort),
        "u32" => sizeof(uint),
        "u64" => sizeof(ulong),
        "usize" => Unsafe.SizeOf<nuint>(),
        "f32" => sizeof(float),
        "f64" => sizeof(double),
        _ => throw new InvalidOperationException($"Size of {type} is not known at compile time")
    };

    public static bool IsNumber(this TypeSymbol type)
        => IsInteger(type)
        || IsFloatingPoint(type);

    public static bool IsInteger(this TypeSymbol type)
        => IsSignedInteger(type)
        || IsUnsignedInteger(type);

    public static bool IsSignedInteger(this TypeSymbol type)
        => type == I8
        || type == I16
        || type == I32
        || type == I64
        || type == ISize;

    public static bool IsUnsignedInteger(this TypeSymbol type)
        => type == U8
        || type == U16
        || type == U32
        || type == U64
        || type == USize;

    public static bool IsFloatingPoint(this TypeSymbol type)
        => type == F32
        || type == F64;
}
