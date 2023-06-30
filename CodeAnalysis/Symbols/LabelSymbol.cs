namespace CodeAnalysis.Symbols;

public sealed record class LabelSymbol(string Name) : Symbol(SymbolKind.Label, Name, BuiltinTypes.Void)
{
    public bool Equals(VariableSymbol? other) => other is not null && Name == other.Name && Type == other.Type;
    public override int GetHashCode() => HashCode.Combine(Name, Type);
    public override string ToString() => Name;
}