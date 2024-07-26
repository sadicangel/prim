using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;

internal sealed record class InterpreterContext(EvaluatedScope EvaluatedScope, Dictionary<LabelSymbol, int> LabelIndices)
{
    private readonly Stack<EvaluatedScope> _scopes = new([EvaluatedScope]);

    public EvaluatedScope EvaluatedScope { get => _scopes.Peek(); }
    public int InstructionIndex { get; set; }
    public PrimValue LastValue { get; set; } = PrimValue.Unit;

    public IDisposable PushScope() => Disposable.EvaluatedScope(this, new EvaluatedScope(EvaluatedScope));

    internal readonly struct Disposable : IDisposable
    {
        private readonly Action _pop;
        private Disposable(Action push, Action pop)
        {
            push();
            _pop = pop;
        }

        public static Disposable EvaluatedScope(InterpreterContext context, EvaluatedScope evaluatedScope) => new(
            () => context._scopes.Push(evaluatedScope),
            () => context._scopes.Pop());

        public void Dispose() => _pop();
    }
}
