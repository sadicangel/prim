using System.Collections.Concurrent;
using System.Collections.Immutable;
using CodeAnalysis.Binding;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Semantic;

internal sealed class BoundModule(ModuleSymbol moduleSymbol)
{
    private readonly record struct BindingResult(BoundNode BoundNode, ImmutableArray<Diagnostic> Diagnostics);

    private readonly ConcurrentDictionary<Symbol, BindingResult> _bindings = [];

    public ModuleSymbol ModuleSymbol { get; } = moduleSymbol;

    public BoundNode GetOrBind(Symbol symbol, out ImmutableArray<Diagnostic> diagnostics) =>
        ((_, diagnostics) = _bindings.GetOrAdd(symbol, Bind)).Item1;

    private BindingResult Bind(Symbol symbol)
    {
        var context = new BindingContext(ModuleSymbol, []);
        var boundNode = symbol.Syntax.Bind(context);
        var diagnostics = context.Diagnostics.Count > 0 ? context.Diagnostics.ToImmutableArray() : [];

        return new BindingResult(boundNode, diagnostics);
    }
}
