namespace CodeAnalysis.Symbols;

public sealed record class VariableSymbol(string Name, bool IsReadOnly, TypeSymbol Type) : Symbol(SymbolKind.Variable, Name, Type)
{
    public bool Equals(VariableSymbol? other) => other is not null && Name == other.Name && Type == other.Type;
    public override int GetHashCode() => HashCode.Combine(Name, Type);
    public override string ToString() => $"{Name}: {Type}";

}