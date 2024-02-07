using CodeAnalysis.Types;
using System.Collections;
using System.Diagnostics;

namespace CodeAnalysis.Evaluation;

public sealed record class Environment : IEnumerable<object?>
{
    private Dictionary<Symbol, object>? _symbols;

    private Environment() { }

    public Environment? Parent { get; init; }

    public bool IsGlobal { get => Parent is null; }

    public Environment CreateChildScope() => new() { Parent = this };

    public void Declare(Symbol symbol, object value)
    {
        if (!(_symbols ??= []).TryAdd(symbol, value))
            throw new UnreachableException($"{symbol.Name} redeclared");
    }

    public object Lookup(Symbol symbol)
    {
        if (_symbols is not null && _symbols.TryGetValue(symbol, out var value))
            return value;

        if (Parent is not null)
            return Parent.Lookup(symbol);

        throw new UnreachableException($"{nameof(Parent)} was null");
    }

    public IEnumerator<object?> GetEnumerator() => _symbols?.Values.GetEnumerator() ?? Enumerable.Empty<object?>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static Environment CreateGlobalScope()
    {
        var scope = new Environment();

        foreach (var symbol in PredefinedSymbols.All)
            scope.Declare(symbol, symbol.Type switch
            {
                TypeType type => type,
                FunctionType func => symbol.Name switch
                {
                    PredefinedSymbolNames.ScanLn => new Func<object>(() => Console.ReadLine() ?? string.Empty),
                    PredefinedSymbolNames.PrintLn => new Func<object, object>(obj =>
                    {
                        Console.WriteLine(obj);
                        return Unit.Value;
                    }),
                    _ => throw new UnreachableException($"Unexpected predefined function '{symbol.Name}'")
                },
                _ => throw new UnreachableException($"Unexpected predefined symbol '{symbol.Name}'")
            });

        return scope;
    }
}