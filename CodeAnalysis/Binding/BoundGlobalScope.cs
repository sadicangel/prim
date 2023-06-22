using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundGlobalScope(IEnumerable<Diagnostic> Diagnostics, IEnumerable<VariableSymbol> Variables, BoundStatement Statement, BoundGlobalScope? Previous = null);
