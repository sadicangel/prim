namespace CodeAnalysis;

public readonly record struct EvaluationResult(object? Value, IEnumerable<Diagnostic> Diagnostics)
{
    public EvaluationResult(object? value) : this(value, Enumerable.Empty<Diagnostic>()) { }
    public EvaluationResult(IEnumerable<Diagnostic> diagnostics) : this(null, diagnostics) { }
}
