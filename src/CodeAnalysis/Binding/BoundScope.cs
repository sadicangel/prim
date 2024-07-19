using System.Collections;
using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;

namespace CodeAnalysis.Binding;

internal class BoundScope(BoundScope? parent = null) : IEnumerable<Symbol>
{
    protected Dictionary<string, Symbol>? Symbols { get; set; }

    public BoundScope Parent { get => parent ?? GlobalBoundScope.Instance; }

    public bool Declare(Symbol symbol) => (Symbols ??= []).TryAdd(symbol.Name, symbol);

    public void Replace(Symbol symbol)
    {
        var scope = this;
        if (scope.Symbols?.ContainsKey(symbol.Name) is true)
        {
            scope.Symbols[symbol.Name] = symbol;
            return;
        }

        do
        {
            scope = scope.Parent;
            if (scope.Symbols?.ContainsKey(symbol.Name) is true)
            {
                scope.Symbols[symbol.Name] = symbol;
                return;
            }
        }
        while (scope != scope.Parent);

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public Symbol? Lookup(string name)
    {
        var scope = this;
        var symbol = scope.Symbols?.GetValueOrDefault(name);
        if (symbol is not null)
            return symbol;

        do
        {
            scope = scope.Parent;
            symbol = scope.Symbols?.GetValueOrDefault(name);
            if (symbol is not null)
                return symbol;
        }
        while (scope != scope.Parent);

        return null;
    }

    public IEnumerator<Symbol> GetEnumerator()
    {
        foreach (var symbol in EnumerateSymbols(this))
            yield return symbol;

        static IEnumerable<Symbol> EnumerateSymbols(BoundScope? scope)
        {
            if (scope is null) yield break;
            if (scope.Symbols is not null)
            {
                foreach (var (_, symbol) in scope.Symbols)
                    yield return symbol;
            }
            foreach (var symbol in EnumerateSymbols(scope.Parent))
                yield return symbol;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class GlobalBoundScope : BoundScope
{
    private static GlobalBoundScope? s_instance;
    public static GlobalBoundScope Instance
    {
        get
        {
            if (s_instance is null)
                Interlocked.CompareExchange(ref s_instance, new GlobalBoundScope(), null);
            return s_instance;
        }
    }

    private GlobalBoundScope() : base()
    {
        Symbols = PredefinedTypes.All().ToDictionary(s => s.Name, s => s);
    }

    public TypeSymbol Any { get => (TypeSymbol)Symbols![PredefinedTypeNames.Any]; }
    public TypeSymbol Unknown { get => (TypeSymbol)Symbols![PredefinedTypeNames.Unknown]; }
    public TypeSymbol Never { get => (TypeSymbol)Symbols![PredefinedTypeNames.Never]; }
    public TypeSymbol Unit { get => (TypeSymbol)Symbols![PredefinedTypeNames.Unit]; }
    public TypeSymbol Type { get => (TypeSymbol)Symbols![PredefinedTypeNames.Type]; }
    public TypeSymbol Str { get => (TypeSymbol)Symbols![PredefinedTypeNames.Str]; }
    public TypeSymbol Bool { get => (TypeSymbol)Symbols![PredefinedTypeNames.Bool]; }
    public TypeSymbol I8 { get => (TypeSymbol)Symbols![PredefinedTypeNames.I8]; }
    public TypeSymbol I16 { get => (TypeSymbol)Symbols![PredefinedTypeNames.I16]; }
    public TypeSymbol I32 { get => (TypeSymbol)Symbols![PredefinedTypeNames.I32]; }
    public TypeSymbol I64 { get => (TypeSymbol)Symbols![PredefinedTypeNames.I64]; }
    public TypeSymbol I128 { get => (TypeSymbol)Symbols![PredefinedTypeNames.I128]; }
    public TypeSymbol ISize { get => (TypeSymbol)Symbols![PredefinedTypeNames.ISize]; }
    public TypeSymbol U8 { get => (TypeSymbol)Symbols![PredefinedTypeNames.U8]; }
    public TypeSymbol U16 { get => (TypeSymbol)Symbols![PredefinedTypeNames.U16]; }
    public TypeSymbol U32 { get => (TypeSymbol)Symbols![PredefinedTypeNames.U32]; }
    public TypeSymbol U64 { get => (TypeSymbol)Symbols![PredefinedTypeNames.U64]; }
    public TypeSymbol U128 { get => (TypeSymbol)Symbols![PredefinedTypeNames.U128]; }
    public TypeSymbol USize { get => (TypeSymbol)Symbols![PredefinedTypeNames.USize]; }
    public TypeSymbol F16 { get => (TypeSymbol)Symbols![PredefinedTypeNames.F16]; }
    public TypeSymbol F32 { get => (TypeSymbol)Symbols![PredefinedTypeNames.F32]; }
    public TypeSymbol F64 { get => (TypeSymbol)Symbols![PredefinedTypeNames.F64]; }
    public TypeSymbol F80 { get => (TypeSymbol)Symbols![PredefinedTypeNames.F80]; }
    public TypeSymbol F128 { get => (TypeSymbol)Symbols![PredefinedTypeNames.F128]; }
}
