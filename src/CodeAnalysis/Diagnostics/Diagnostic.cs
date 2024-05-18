using CodeAnalysis.Text;

namespace CodeAnalysis.Diagnostics;
public sealed record class Diagnostic(string Id, SourceLocation Location, DiagnosticSeverity Severity, string Message);
