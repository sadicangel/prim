namespace CodeAnalysis.Binding;

internal sealed record class BoundGlobalScope(IEnumerable<Diagnostic> Diagnostics, IEnumerable<Variable> Variables, BoundStatement Statement, BoundGlobalScope? Previous = null);
