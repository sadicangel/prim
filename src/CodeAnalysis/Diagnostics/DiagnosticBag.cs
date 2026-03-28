using System.Collections;
using System.Diagnostics;
using CodeAnalysis.Text;

namespace CodeAnalysis.Diagnostics;

[DebuggerDisplay("Count = {Count}")]
public sealed class DiagnosticBag : IReadOnlyList<Diagnostic>, IDiagnosticReporter
{
    private readonly List<Diagnostic> _diagnostics;

    public DiagnosticBag() => _diagnostics = [];
    public DiagnosticBag(IEnumerable<Diagnostic> diagnostics) => _diagnostics = [.. diagnostics];

    public int Count { get => _diagnostics.Count; }

    public bool HasErrorDiagnostics { get => _diagnostics.Any(d => d.Severity is DiagnosticSeverity.Error); }

    public bool HasWarningDiagnostics { get => _diagnostics.Any(d => d.Severity is DiagnosticSeverity.Warning); }

    public bool HasInformationDiagnostics { get => _diagnostics.Any(d => d.Severity is DiagnosticSeverity.Information); }

    public Diagnostic this[int index] { get => _diagnostics[index]; }

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddRange(IEnumerable<Diagnostic> diagnostics) => _diagnostics.AddRange(diagnostics);

    public IEnumerable<Diagnostic> GetDiagnostics() => _diagnostics;

    public void Report(SourceSpan sourceSpan, DiagnosticSeverity severity, string message)
    {
        _diagnostics.Add(new Diagnostic(Id: "", sourceSpan, severity, message));
    }
}
