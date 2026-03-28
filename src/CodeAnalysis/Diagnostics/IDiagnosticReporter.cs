using CodeAnalysis.Text;

namespace CodeAnalysis.Diagnostics;

internal interface IDiagnosticReporter
{
    public IEnumerable<Diagnostic> GetDiagnostics();

    void Report(SourceSpan sourceSpan, DiagnosticSeverity severity, string message);
}
