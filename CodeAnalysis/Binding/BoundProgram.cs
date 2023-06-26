namespace CodeAnalysis.Binding;

internal sealed record class BoundProgram(BoundStatement Statement, IReadOnlyList<Diagnostic> Diagnostics);
