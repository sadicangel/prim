using CodeAnalysis.Interpretation;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis;
public sealed class Evaluation
{
    private Evaluation(Compilation compilation, Evaluation? previous = null)
    {
        Compilation = compilation;
        Previous = previous;
        EvaluatedScope = previous?.EvaluatedScope ?? ModuleValue.CreateGlobalModule(compilation.BoundScope);
        Values = new(Compilation.BoundTrees.Select(tree => Interpreter.Evaluate(tree, EvaluatedScope)));
    }

    public Compilation Compilation { get; }
    public Evaluation? Previous { get; }
    internal ScopeValue? EvaluatedScope { get; }
    internal ReadOnlyList<PrimValue> Values { get; }

    public static Evaluation Evaluate(Compilation compilation, Evaluation? previous = null) => new(compilation, previous);
}
