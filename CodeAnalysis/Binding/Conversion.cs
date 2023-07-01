using CodeAnalysis.Symbols;

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
        if (to == BuiltinTypes.I8 && from.IsNumber())
        {
            return Explicit;
        }

        if (to == BuiltinTypes.I16 && from.IsNumber())
        {
            if (from == BuiltinTypes.I8 ||
                from == BuiltinTypes.U8)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.I32 && from.IsNumber())
        {
            if (from == BuiltinTypes.I8 ||
                from == BuiltinTypes.U8 ||
                from == BuiltinTypes.I16 ||
                from == BuiltinTypes.U16)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.I64 && from.IsNumber())
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

        if (to == BuiltinTypes.ISize && from.IsNumber())
        {
            if (from == BuiltinTypes.I8 ||
                from == BuiltinTypes.U8 ||
                from == BuiltinTypes.I16 ||
                from == BuiltinTypes.U16)
                return Implicit;
            if ((from == BuiltinTypes.I32 ||
                from == BuiltinTypes.U32 ||
                from == BuiltinTypes.I64)
                && from.SizeOf() <= to.SizeOf())
                return Implicit;
            return Explicit;
        }

        // Unsigned integers
        if (to == BuiltinTypes.U8 && from.IsNumber())
        {
            return Explicit;
        }

        if (to == BuiltinTypes.U16 && from.IsNumber())
        {
            if (from == BuiltinTypes.U8)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.U32 && from.IsNumber())
        {
            if (from == BuiltinTypes.U8 ||
                from == BuiltinTypes.U16)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.U64 && from.IsNumber())
        {
            if (from == BuiltinTypes.U8 ||
                from == BuiltinTypes.U16 ||
                from == BuiltinTypes.U32)
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.USize && from.IsNumber())
        {
            if (from == BuiltinTypes.U8 ||
                from == BuiltinTypes.U16)
                return Implicit;
            if ((from == BuiltinTypes.U32 ||
                from == BuiltinTypes.U64)
                && from.SizeOf() <= to.SizeOf())
                return Implicit;
            return Explicit;
        }

        // Floating point
        if (to == BuiltinTypes.F32 && from.IsNumber())
        {
            if (from.IsInteger())
                return Implicit;
            return Explicit;
        }

        if (to == BuiltinTypes.F64 && from.IsNumber())
        {
            return Implicit;
        }

        return None;
    }
}
