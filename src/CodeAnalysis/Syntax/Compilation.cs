using CodeAnalysis.Binding;
using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;
public readonly record struct EvaluationResult(object? Value, IReadOnlyList<Diagnostic> Diagnostics);

public sealed record class Compilation(IReadOnlyList<SyntaxTree> SyntaxTrees, Compilation? Previous = null)
{
    public object? Evaluate()
    {
        if (SyntaxTrees.Any(st => st.Diagnostics.HasErrors))
            return new EvaluationResult(null, [.. SyntaxTrees.SelectMany(tree => tree.Diagnostics)]);

        var program = Binder.Bind(Scope.CreateGlobalScope(), SyntaxTrees[0].Root);
        if (program.Diagnostics.HasErrors)
            return new EvaluationResult(null, program.Diagnostics);

        //var evaluator = new Evaluator(program, globals);
        //var value = evaluator.Evaluate();

        //return new EvaluationResult(value, diagnostics);

        return null;
    }
}