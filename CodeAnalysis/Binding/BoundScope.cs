using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Binding;

internal sealed class BoundScope
{
    private readonly BoundScope? _parent;
    private readonly Dictionary<string, Variable> _variables = new();

    public BoundScope(BoundScope? parent)
    {
        _parent = parent;
    }

    public IEnumerable<Variable> DeclaredVariables { get => _variables.Values; }

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

        return _parent is not null && _parent.TryLookup(name, out variable);
    }
}
