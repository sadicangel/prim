using CodeAnalysis.Text;

namespace CodeAnalysis;

public enum DiagnosticSeverity { Error, Warning, Information }

public sealed record class Diagnostic(DiagnosticSeverity Severity, TextLocation Location, string Message)
{
    public override string ToString() => Message;
}