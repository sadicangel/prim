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

        if (from == BuiltinTypes.I32 && to == BuiltinTypes.F32)
            return Explicit;

        if (from == BuiltinTypes.F32 && to == BuiltinTypes.I32)
            return Explicit;

        return None;
    }
}
