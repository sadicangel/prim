using CodeAnalysis.Text;

namespace CodeAnalysis.Diagnostics;
public sealed record class Diagnostic(string Id, SourceSpan SourceSpan, DiagnosticSeverity Severity, string Message);
