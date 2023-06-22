using CodeAnalysis.Symbols;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Binding;

internal sealed record class BoundScope(BoundScope? Parent, IReadOnlyCollection<VariableSymbol>? Variables = null, IReadOnlyCollection<FunctionSymbol>? Functions = null)
{
    private Dictionary<string, VariableSymbol>? _variables = Variables?.ToDictionary(v => v.Name);
    private Dictionary<string, FunctionSymbol>? _functions = Functions?.ToDictionary(v => v.Name);

    public IReadOnlyCollection<VariableSymbol> Variables { get => _variables?.Values ?? (IReadOnlyCollection<VariableSymbol>)Array.Empty<VariableSymbol>(); }

    public IReadOnlyCollection<FunctionSymbol> Functions { get => _functions?.Values ?? (IReadOnlyCollection<FunctionSymbol>)Array.Empty<FunctionSymbol>(); }

    public bool TryDeclare(VariableSymbol variable) => (_variables ??= new()).TryAdd(variable.Name, variable);

    public bool TryDeclare(VariableSymbol variable, [MaybeNullWhen(true)] out VariableSymbol existingVariable)
    {
        if ((_variables ??= new()).TryGetValue(variable.Name, out existingVariable))
            return false;

        _variables[variable.Name] = variable;
        return true;
    }

    public bool TryLookup(string name, [MaybeNullWhen(false)] out VariableSymbol variable)
    {
        variable = null;

        if (_variables is not null && _variables.TryGetValue(name, out variable))
            return true;

        return Parent is not null && Parent.TryLookup(name, out variable);
    }

    public bool TryDeclare(FunctionSymbol function) => (_functions ??= new()).TryAdd(function.Name, function);

    public bool TryDeclare(FunctionSymbol function, [MaybeNullWhen(true)] out FunctionSymbol existingFunction)
    {
        if ((_functions ??= new()).TryGetValue(function.Name, out existingFunction))
            return false;

        _functions[function.Name] = function;
        return true;
    }

    public bool TryLookup(string name, [MaybeNullWhen(false)] out FunctionSymbol function)
    {
        function = null;

        if (_functions is not null && _functions.TryGetValue(name, out function))
            return true;

        return Parent is not null && Parent.TryLookup(name, out function);
    }
}
