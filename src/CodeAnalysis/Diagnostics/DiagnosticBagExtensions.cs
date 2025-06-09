namespace CodeAnalysis.Diagnostics;

public static class DiagnosticBagExtensions
{
    extension(IEnumerable<Diagnostic> diagnostics)
    {
        public bool HasErrorDiagnostics => diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);
        public bool HasWarningDiagnostics => diagnostics.Any(d => d.Severity == DiagnosticSeverity.Warning);
    }
}
