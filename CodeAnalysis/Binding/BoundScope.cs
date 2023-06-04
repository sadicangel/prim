using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Binding;

internal sealed record class BoundScope(BoundScope? Parent, IReadOnlyCollection<Variable>? Variables = null)
{
    private readonly Dictionary<string, Variable> _variables = Variables?.ToDictionary(v => v.Name) ?? new();


    public IReadOnlyCollection<Variable> Variables { get => _variables.Values; }

    public bool TryDeclare(Variable variable) => _variables.TryAdd(variable.Name, variable);

    public bool TryDeclare(Variable variable, [MaybeNullWhen(true)] out Variable existingVariable)
    {
        if (_variables.TryGetValue(variable.Name, out existingVariable))
            return false;

        _variables[variable.Name] = variable;
        return true;
    }

    public bool TryLookup(string name, [MaybeNullWhen(false)] out Variable variable)
    {
        if (_variables.TryGetValue(name, out variable))
            return true;

        return Parent is not null && Parent.TryLookup(name, out variable);
    }
}
