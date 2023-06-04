namespace CodeAnalysis;

public sealed record class Variable(string Name, bool IsReadOnly, Type Type)
{
    public bool Equals(Variable? other) => other?.Name == Name;

    public override int GetHashCode() => Name.GetHashCode();
}