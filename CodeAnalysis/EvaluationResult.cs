namespace CodeAnalysis;

public sealed record class EvaluationResult(object? Value, IReadOnlyList<Diagnostic> Diagnostics)
{
    public EvaluationResult(object? value) : this(value, Array.Empty<Diagnostic>()) { }
    public EvaluationResult(IReadOnlyList<Diagnostic> diagnostics) : this(null, diagnostics) { }
}
