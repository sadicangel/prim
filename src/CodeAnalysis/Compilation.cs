using CodeAnalysis.Binding;
using CodeAnalysis.Evaluation;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using CodeAnalysis.Types;
using Spectre.Console;
using Environment = CodeAnalysis.Evaluation.Environment;

namespace CodeAnalysis;
public readonly record struct EvaluationResult(object Value, PrimType Type, DiagnosticBag Diagnostics)
{
    public bool HasDiagnostics => Diagnostics.Count > 0;
    public EvaluationResult(object value, PrimType type) : this(value, type, []) { }
    public EvaluationResult(DiagnosticBag diagnostics) : this(Unit.Value, PredefinedTypes.Unit, diagnostics) { }
}

public sealed record class Compilation(IReadOnlyList<SyntaxTree> SyntaxTrees, Compilation? Previous = null)
{
    public EvaluationResult Evaluate()
    {
        if (SyntaxTrees.Any(st => st.Diagnostics.HasErrors))
            return new EvaluationResult(new DiagnosticBag(SyntaxTrees.SelectMany(tree => tree.Diagnostics)));

        var program = Binder.Bind(Scope.CreateGlobalScope(), SyntaxTrees[0].Root);
        if (program.Diagnostics.HasErrors)
            return new EvaluationResult(program.Diagnostics);

        AnsiConsole.Write(program.ToRenderable());
        AnsiConsole.WriteLine();

        return Evaluator.Evaluate(Environment.CreateGlobalScope(), program);
    }
}
