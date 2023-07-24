using CodeAnalysis.Binding.Statements;

namespace CodeAnalysis.Binding;

internal sealed record class BoundProgram(BoundBlockStatement? Statement, IReadOnlyDiagnosticBag Diagnostics);