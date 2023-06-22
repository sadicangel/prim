namespace CodeAnalysis.Symbols;
public sealed record class FunctionSymbol(string Name, TypeSymbol Type, params ParameterSymbol[] Parameters) : Symbol(Name, SymbolKind.Function)
{
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
