namespace CodeAnalysis.Diagnostics;

internal interface IDiagnosticReporter
{
    public IEnumerable<Diagnostic> GetDiagnostics();

    void Report(Diagnostic diagnostic);
}
