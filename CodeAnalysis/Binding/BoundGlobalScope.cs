using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed class BoundGlobalScope
{
    public BoundGlobalScope(IEnumerable<Diagnostic> diagnostics, IEnumerable<Variable> variables, BoundExpression expression, BoundGlobalScope? previous = null)
    {
        Diagnostics = diagnostics;
        Variables = variables;
        Expression = expression;
        Previous = previous;
    }

    public IEnumerable<Diagnostic> Diagnostics { get; }
    public IEnumerable<Variable> Variables { get; }
    public BoundExpression Expression { get; }
    public BoundGlobalScope? Previous { get; }
}
