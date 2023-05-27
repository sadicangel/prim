using CodeAnalysis.Binding;
using CodeAnalysis.Syntax;

namespace CodeAnalysis;

public sealed record class EvaluationResult(object? Value, IEnumerable<Diagnostic> Diagnostics);

public sealed class Compilation
{
    public Compilation(SyntaxTree syntaxTree)
    {
        SyntaxTree = syntaxTree;
    }

    public SyntaxTree SyntaxTree { get; }

    public EvaluationResult Evaluate(Dictionary<Variable, object> variables)
    {
        var binder = new Binder(variables);
        var boundExpression = binder.BindExpression(SyntaxTree.Root);

        var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();
        if (diagnostics.Any())
        {
            return new EvaluationResult(null, diagnostics);
        }
        var evaluator = new Evaluator(boundExpression, variables);
        var value = evaluator.Evaluate();

        return new EvaluationResult(value, Enumerable.Empty<Diagnostic>());
    }
}