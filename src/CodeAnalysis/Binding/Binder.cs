using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binding;

internal abstract class Binder(Binder? parent) : ISymbolScope, IDiagnosticReporter
{
    private readonly DiagnosticBag _diagnostics = parent?._diagnostics ?? [];

    protected Binder? Parent => parent;

    public abstract ModuleSymbol Module { get; }

    /// <inheritdoc />
    public abstract bool TryDeclare(Symbol symbol);

    /// <inheritdoc />
    public virtual bool TryLookup<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol) where TSymbol : Symbol
    {
        if (TryLookupInCurrentScope(name, out symbol)) return true;
        return parent?.TryLookup(name, out symbol) is true;
    }

    protected abstract bool TryLookupInCurrentScope<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol) where TSymbol : Symbol;

    /// <inheritdoc />
    public IEnumerable<Diagnostic> GetDiagnostics() => _diagnostics;

    /// <inheritdoc />
    public void Report(SourceSpan sourceSpan, DiagnosticSeverity severity, string message) =>
        _diagnostics.Report(sourceSpan, severity, message);

    public void AddDiagnostics(IEnumerable<Diagnostic> diagnostics) => _diagnostics.AddRange(diagnostics);
}
