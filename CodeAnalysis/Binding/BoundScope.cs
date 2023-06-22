using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundScope(BoundScope? Parent, IReadOnlyCollection<VariableSymbol>? Variables = null)
{
    private readonly Dictionary<string, VariableSymbol> _variables = Variables?.ToDictionary(v => v.Name) ?? new();


    public IReadOnlyCollection<VariableSymbol> Variables { get => _variables.Values; }

    public bool TryDeclare(VariableSymbol variable) => _variables.TryAdd(variable.Name, variable);

    public bool TryDeclare(VariableSymbol variable, [MaybeNullWhen(true)] out VariableSymbol existingVariable)
    {
        if (_variables.TryGetValue(variable.Name, out existingVariable))
            return false;

        _variables[variable.Name] = variable;
        return true;
    }

    public bool TryLookup(string name, [MaybeNullWhen(false)] out VariableSymbol variable)
    {
        if (_variables.TryGetValue(name, out variable))
            return true;

        return Parent is not null && Parent.TryLookup(name, out variable);
    }
}
