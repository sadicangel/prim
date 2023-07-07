using CodeAnalysis.Binding;
using CodeAnalysis.Lowering;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using System.Diagnostics;

namespace CodeAnalysis;

public sealed class Compilation
{
    private BoundGlobalScope? _globalScope;

    public Compilation(params SyntaxTree[] syntaxTrees) : this(previous: null, syntaxTrees) { }

    public Compilation(Compilation? previous, params SyntaxTree[] syntaxTrees)
    {
        SyntaxTrees = syntaxTrees;
        Previous = previous;
    }

    public IReadOnlyList<SyntaxTree> SyntaxTrees { get; }

    public Compilation? Previous { get; }

    private BoundGlobalScope GetOrCreateGlobalScope()
    {
        if (_globalScope is null)
            Interlocked.CompareExchange(ref _globalScope, Binder.BindGlobalScope(SyntaxTrees, Previous?._globalScope), comparand: null);
        return _globalScope;
    }

    public EvaluationResult Evaluate(Dictionary<Symbol, object?> globals)
    {
        var diagnostics = SyntaxTrees.SelectMany(tree => tree.Diagnostics).Concat(GetOrCreateGlobalScope().Diagnostics);
        if (diagnostics.Any())
            return new EvaluationResult(diagnostics);

        var program = Binder.BindProgram(GetOrCreateGlobalScope());

        if (program.Diagnostics.Any())
            return new EvaluationResult(program.Diagnostics);

        var lowered = Lowerer.Lower(program);

        Debug.WriteLine(lowered.Statement);

        var evaluator = new Evaluator(lowered, globals);
        var value = evaluator.Evaluate();

        return new EvaluationResult(value);
    }

    public void WriteTo(TextWriter writer)
    {
        var lowered = Lowerer.Lower(Binder.BindProgram(GetOrCreateGlobalScope()));
        lowered.Statement.WriteTo(writer);
    }
}