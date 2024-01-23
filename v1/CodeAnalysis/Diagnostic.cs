using CodeAnalysis.Text;

namespace CodeAnalysis;

public enum DiagnosticSeverity { Error, Warning }

public sealed record class Diagnostic(DiagnosticSeverity Severity, TextLocation Location, string Message)
{
    public override string ToString() => Message;
}