using CodeAnalysis.Binding;
using CodeAnalysis.Syntax;

namespace CodeAnalysis;

public sealed record class EvaluationResult(object? Value, IReadOnlyList<Diagnostic> Diagnostics);

public sealed class Compilation
{
    private BoundGlobalScope? _globalScope;

    public Compilation(SyntaxTree syntaxTree, Compilation? previous = null)
    {
        SyntaxTree = syntaxTree;
        Previous = previous;
    }

    public SyntaxTree SyntaxTree { get; }
    public Compilation? Previous { get; }

    internal BoundGlobalScope GlobalScope
    {
        get
        {
            if (_globalScope is null)
                Interlocked.CompareExchange(ref _globalScope, Binder.BindGlobalScope(SyntaxTree.Root, Previous?.GlobalScope), null);
            return _globalScope;
        }
    }

    public EvaluationResult Evaluate(Dictionary<Variable, object> variables)
    {
        var boundStatement = GlobalScope.Statement;

        var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToArray();
        if (diagnostics.Any())
        {
            return new EvaluationResult(null, diagnostics);
        }
        var evaluator = new Evaluator(boundStatement, variables);
        var value = evaluator.Evaluate();

        return new EvaluationResult(value, Array.Empty<Diagnostic>());
    }
}