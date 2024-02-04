using CodeAnalysis.Binding;
using CodeAnalysis.Text;
using Spectre.Console;

namespace CodeAnalysis.Syntax;
public readonly record struct EvaluationResult(object? Value, DiagnosticBag Diagnostics);

public sealed record class Compilation(IReadOnlyList<SyntaxTree> SyntaxTrees, Compilation? Previous = null)
{
    public EvaluationResult Evaluate()
    {
        if (SyntaxTrees.Any(st => st.Diagnostics.HasErrors))
            return new EvaluationResult(null, new DiagnosticBag(SyntaxTrees.SelectMany(tree => tree.Diagnostics)));

        var program = Binder.Bind(Scope.CreateGlobalScope(), SyntaxTrees[0].Root);
        if (program.Diagnostics.HasErrors)
            return new EvaluationResult(null, program.Diagnostics);

        AnsiConsole.Write(program.ToRenderable());

        //var evaluator = new Evaluator(program, globals);
        //var value = evaluator.Evaluate();

        //return new EvaluationResult(value, diagnostics);

        return new EvaluationResult(null, []);
    }
}