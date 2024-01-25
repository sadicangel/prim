namespace CodeAnalysis.Text;

public enum DiagnosticSeverity { Error, Warning, Information }

public sealed record class Diagnostic(DiagnosticSeverity Severity, SourceLocation Location, string Message)
{
    public override string ToString() => Message;
}