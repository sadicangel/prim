using System.Collections.Immutable;
using CodeAnalysis.Binding;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Semantic;

internal sealed class BoundProgram(VariableSymbol entryPoint, DiagnosticBag diagnostics)
{
    private readonly DiagnosticBag _diagnostics = diagnostics;

    public BoundExpression EntryPoint => field ??= (BoundExpression)entryPoint.Syntax.Bind(new BindingContext(entryPoint.ContainingModule, _diagnostics));
    public ImmutableArray<Diagnostic> Diagnostics => [.. _diagnostics];
}
