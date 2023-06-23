namespace CodeAnalysis.Symbols;

public sealed record class ParameterSymbol(string Name, TypeSymbol Type) : Symbol(SymbolKind.Parameter, Name, Type)
{
    public bool Equals(ParameterSymbol? other) => other is not null && Name == other.Name && Type == other.Type;
    public override int GetHashCode() => HashCode.Combine(Name, Type);

    public override string ToString() => $"{Name}: {Type}";
}