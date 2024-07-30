using System.Collections;
using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;

internal class EvaluatedScope(EvaluatedScope? parent = null) : IEnumerable<PrimValue>
{
    protected Dictionary<Symbol, PrimValue>? Values { get; set; }

    public EvaluatedScope Parent { get => parent ?? GlobalEvaluatedScope.Instance; }

    public void Declare(Symbol symbol, PrimValue value)
    {
        if (!(Values ??= []).TryAdd(symbol, value))
            throw new UnreachableException(DiagnosticMessage.SymbolRedeclaration(symbol.Name));
    }

    public void Replace(Symbol symbol, PrimValue value)
    {
        var scope = this;
        if (scope.Values?.ContainsKey(symbol) is true)
        {
            scope.Values[symbol] = value;
            return;
        }

        do
        {
            scope = scope.Parent;
            if (scope.Values?.ContainsKey(symbol) is true)
            {
                scope.Values[symbol] = value;
                return;
            }
        }
        while (scope != scope.Parent);

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public PrimValue Lookup(Symbol symbol)
    {
        var scope = this;
        var value = scope.Values?.GetValueOrDefault(symbol);
        if (value is not null)
            return value;

        do
        {
            scope = scope.Parent;
            value = scope.Values?.GetValueOrDefault(symbol);
            if (value is not null)
                return value;
        }
        while (scope != scope.Parent);

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public IEnumerator<PrimValue> GetEnumerator()
    {
        foreach (var value in EnumerateValues(this))
            yield return value;

        static IEnumerable<PrimValue> EnumerateValues(EvaluatedScope? scope)
        {
            if (scope is null) yield break;
            if (scope.Values is not null)
            {
                foreach (var (_, value) in scope.Values)
                    yield return value;
            }
            foreach (var value in EnumerateValues(scope.Parent))
                yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
