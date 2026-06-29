using CodeAnalysis.Text;

namespace CodeAnalysis.Diagnostics;

public sealed record class Diagnostic(DiagnosticId Id, DiagnosticSeverity Severity, SourceSpan SourceSpan, string Message);
