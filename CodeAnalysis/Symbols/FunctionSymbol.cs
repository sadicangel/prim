namespace CodeAnalysis.Symbols;
public sealed record class FunctionSymbol(string Name, TypeSymbol Type, IReadOnlyList<ParameterSymbol> Parameters) : Symbol(SymbolKind.Function, Name, Type)
{
    public FunctionSymbol(string name, TypeSymbol type, params ParameterSymbol[] parameters) : this(name, type, (IReadOnlyList<ParameterSymbol>)parameters) { }

    public bool Equals(FunctionSymbol? other) => other is not null && Name == other.Name && Type == other.Type && Parameters.SequenceEqual(other.Parameters);
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Name);
        hash.Add(Type);
        foreach (var parameter in Parameters)
            hash.Add(parameter);
        return hash.ToHashCode();
    }

    public override string ToString() => $"{Name}: ({String.Join(", ", Parameters.Select(p => p.ToString()))}) => {Type}";
}
