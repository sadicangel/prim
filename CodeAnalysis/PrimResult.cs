namespace CodeAnalysis;

public readonly record struct PrimResult<T>(T Value, IReadOnlyDiagnosticBag Diagnostics)
{
    public bool HasDiagnostics { get => Diagnostics.Count > 0; }
}