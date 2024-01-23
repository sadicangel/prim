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
        if (from == PredefinedTypes.Never || to == PredefinedTypes.Never)
            return None;

        if (from == to)
            return Identity;

        if (to == PredefinedTypes.Any)
            return Implicit;

        // Signed integers
        if (to == PredefinedTypes.I8 && from.IsNumber)
        {
            return Explicit;
        }

        if (to == PredefinedTypes.I16 && from.IsNumber)
        {
            if (from == PredefinedTypes.I8 ||
                from == PredefinedTypes.U8)
                return Implicit;
            return Explicit;
        }

        if (to == PredefinedTypes.I32 && from.IsNumber)
        {
            if (from == PredefinedTypes.I8 ||
                from == PredefinedTypes.U8 ||
                from == PredefinedTypes.I16 ||
                from == PredefinedTypes.U16)
                return Implicit;
            return Explicit;
        }

        if (to == PredefinedTypes.I64 && from.IsNumber)
        {
            if (from == PredefinedTypes.I8 ||
                from == PredefinedTypes.U8 ||
                from == PredefinedTypes.I16 ||
                from == PredefinedTypes.U16 ||
                from == PredefinedTypes.I32 ||
                from == PredefinedTypes.U32)
                return Implicit;
            return Explicit;
        }

        if (to == PredefinedTypes.ISize && from.IsNumber)
        {
            if (from == PredefinedTypes.I8 ||
                from == PredefinedTypes.U8 ||
                from == PredefinedTypes.I16 ||
                from == PredefinedTypes.U16)
                return Implicit;
            if ((from == PredefinedTypes.I32 ||
                from == PredefinedTypes.U32 ||
                from == PredefinedTypes.I64)
                && from.BinarySize <= to.BinarySize)
                return Implicit;
            return Explicit;
        }

        // Unsigned integers
        if (to == PredefinedTypes.U8 && from.IsNumber)
        {
            return Explicit;
        }

        if (to == PredefinedTypes.U16 && from.IsNumber)
        {
            if (from == PredefinedTypes.U8)
                return Implicit;
            return Explicit;
        }

        if (to == PredefinedTypes.U32 && from.IsNumber)
        {
            if (from == PredefinedTypes.U8 ||
                from == PredefinedTypes.U16)
                return Implicit;
            return Explicit;
        }

        if (to == PredefinedTypes.U64 && from.IsNumber)
        {
            if (from == PredefinedTypes.U8 ||
                from == PredefinedTypes.U16 ||
                from == PredefinedTypes.U32)
                return Implicit;
            return Explicit;
        }

        if (to == PredefinedTypes.USize && from.IsNumber)
        {
            if (from == PredefinedTypes.U8 ||
                from == PredefinedTypes.U16)
                return Implicit;
            if ((from == PredefinedTypes.U32 ||
                from == PredefinedTypes.U64)
                && from.BinarySize <= to.BinarySize)
                return Implicit;
            return Explicit;
        }

        // Floating point
        if (to == PredefinedTypes.F32 && from.IsNumber)
        {
            if (from.IsInteger)
                return Implicit;
            return Explicit;
        }

        if (to == PredefinedTypes.F64 && from.IsNumber)
        {
            return Implicit;
        }

        return None;
    }
}
