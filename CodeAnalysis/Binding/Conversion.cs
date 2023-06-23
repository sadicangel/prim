using CodeAnalysis.Symbols;
using System.Runtime.CompilerServices;

namespace CodeAnalysis.Binding;
public sealed record class Conversion(bool Exists, bool IsIdentity, bool IsImplicit)
{
    public static readonly Conversion None = new(Exists: false, IsIdentity: false, IsImplicit: false);
    public static readonly Conversion Identity = new(Exists: true, IsIdentity: true, IsImplicit: true);
    public static readonly Conversion Implicit = new(Exists: true, IsIdentity: false, IsImplicit: true);
    public static readonly Conversion Explicit = new(Exists: true, IsIdentity: false, IsImplicit: false);

    public bool IsExplicit { get => Exists && !IsImplicit; }

    public static Conversion Classify(TypeSymbol from, TypeSymbol to)
    {
        if (from == BuiltinTypes.Never || to == BuiltinTypes.Never)
            return None;

        if (from == to)
            return Identity;

        if (to == BuiltinTypes.Any)
            return Implicit;

        // Signed integers
        if (to == BuiltinTypes.I8 && IsNumber(from))
        {
            return Explicit;
        }

        if (to == BuiltinTypes.I16 && IsNumber(from))
        {
            if (from == BuiltinTypes.I8 ||
                from == BuiltinTypes.U8)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.I32 && IsNumber(from))
        {
            if (from == BuiltinTypes.I8 ||
                from == BuiltinTypes.U8 ||
                from == BuiltinTypes.I16 ||
                from == BuiltinTypes.U16)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.I64 && IsNumber(from))
        {
            if (from == BuiltinTypes.I8 ||
                from == BuiltinTypes.U8 ||
                from == BuiltinTypes.I16 ||
                from == BuiltinTypes.U16 ||
                from == BuiltinTypes.I32 ||
                from == BuiltinTypes.U32)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.ISize && IsNumber(from))
        {
            if (from == BuiltinTypes.I8 ||
                from == BuiltinTypes.U8 ||
                from == BuiltinTypes.I16 ||
                from == BuiltinTypes.U16)
                return Implicit;
            if ((from == BuiltinTypes.I32 ||
                from == BuiltinTypes.U32 ||
                from == BuiltinTypes.I64)
                && SizeOf(from) <= SizeOf(to))
                return Implicit;
            return Explicit;
        }

        // Unsigned integers
        if (to == BuiltinTypes.U8 && IsNumber(from))
        {
            return Explicit;
        }

        if (to == BuiltinTypes.U16 && IsNumber(from))
        {
            if (from == BuiltinTypes.U8)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.U32 && IsNumber(from))
        {
            if (from == BuiltinTypes.U8 ||
                from == BuiltinTypes.U16)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.U64 && IsNumber(from))
        {
            if (from == BuiltinTypes.U8 ||
                from == BuiltinTypes.U16 ||
                from == BuiltinTypes.U32)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.USize && IsNumber(from))
        {
            if (from == BuiltinTypes.U8 ||
                from == BuiltinTypes.U16)
                return Implicit;
            if ((from == BuiltinTypes.U32 ||
                from == BuiltinTypes.U64)
                && SizeOf(from) <= SizeOf(to))
                return Implicit;
            return Explicit;
        }

        // Floating point
        if (to == BuiltinTypes.F32 && IsNumber(from))
        {
            if (IsInteger(from))
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.F64 && IsNumber(from))
        {
            return Implicit;
        }

        return None;
    }

    private static int SizeOf(TypeSymbol type) => type.Name switch
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

    private static bool IsNumber(TypeSymbol type)
        => IsInteger(type)
        || IsFloatingPoint(type);

    private static bool IsInteger(TypeSymbol type)
        => IsSignedInteger(type)
        || IsUnsignedInteger(type);

    private static bool IsSignedInteger(TypeSymbol type)
        => type == BuiltinTypes.I8
        || type == BuiltinTypes.I16
        || type == BuiltinTypes.I32
        || type == BuiltinTypes.I64
        || type == BuiltinTypes.ISize;

    private static bool IsUnsignedInteger(TypeSymbol type)
        => type == BuiltinTypes.U8
        || type == BuiltinTypes.U16
        || type == BuiltinTypes.U32
        || type == BuiltinTypes.U64
        || type == BuiltinTypes.USize;

    private static bool IsFloatingPoint(TypeSymbol type)
        => type == BuiltinTypes.F32
        || type == BuiltinTypes.F64;
}
