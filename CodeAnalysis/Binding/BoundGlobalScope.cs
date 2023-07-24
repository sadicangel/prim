using CodeAnalysis.Binding.Statements;
using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundGlobalScope(IReadOnlyList<BoundStatement> Statements, IEnumerable<Symbol> Symbols, IReadOnlyDiagnosticBag Diagnostics, BoundGlobalScope? Previous = null)
{
    public IEnumerable<FunctionSymbol> Functions { get => Symbols.OfType<FunctionSymbol>(); }
    public IEnumerable<VariableSymbol> Variables { get => Symbols.OfType<VariableSymbol>(); }
    public IEnumerable<TypeSymbol> Types { get => Symbols.OfType<TypeSymbol>(); }
}
