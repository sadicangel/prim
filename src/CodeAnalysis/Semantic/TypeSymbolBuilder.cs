namespace CodeAnalysis.Semantic;

internal readonly struct TypeSymbolBuilder(ModuleSymbol global)
{
    private static readonly IEqualityComparer<TypeSymbol> s_comparer = EqualityComparer<TypeSymbol>.Create(
        equals: (x, y) => x is null ? y is null : x.Name == y?.Name,
        getHashCode: obj => obj.Name.GetHashCode());

    private readonly HashSet<TypeSymbol> _types = new(s_comparer);

    public TypeSymbolBuilder Add(params ReadOnlySpan<TypeSymbol> types)
    {
        foreach (var type in types)
        {
            if (type.IsNever) _types.Clear();
            _types.Add(type);
        }

        return this;
    }

    public TypeSymbolBuilder AddRange(IEnumerable<TypeSymbol> types)
    {
        foreach (var type in types)
        {
            if (type.IsNever) _types.Clear();
            _types.Add(type);
        }

        return this;
    }

    public TypeSymbol Build() => _types.Count switch
    {
        0 => global.UnknownType,
        1 => _types.Single(),
        _ => new UnionTypeSymbol([.. _types]),
    };
}
