namespace CodeAnalysis.Symbols;

public sealed record class VariableSymbol(string Name, bool IsReadOnly, TypeSymbol Type) : Symbol(Name, SymbolKind.Variable)
{
    public bool Equals(VariableSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}