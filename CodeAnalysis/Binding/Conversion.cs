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
        if (from == TypeSymbol.Never || to == TypeSymbol.Never)
            return None;

        if (from == to)
            return Identity;

        if (to == TypeSymbol.Any)
            return Implicit;

        if (from == TypeSymbol.I32 && to == TypeSymbol.F32)
            return Explicit;

        if (from == TypeSymbol.F32 && to == TypeSymbol.I32)
            return Explicit;

        return None;
    }
}
