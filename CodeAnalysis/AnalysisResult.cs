namespace CodeAnalysis;

public readonly record struct AnalysisResult<T>(T Value, IReadOnlyList<Diagnostic> Diagnostics)
{
    public bool HasDiagnostics { get => Diagnostics.Count > 0; }

    public bool HasErrors { get => Diagnostics.Any(d => d.IsError); }

    public static implicit operator AnalysisResult<T>(T value) => new(value, Array.Empty<Diagnostic>());
}