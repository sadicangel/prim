using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;

namespace CodeAnalysis.Binding;

internal sealed record class BinderContext(BoundTree BoundTree, BoundScope BoundScope)
{
    private readonly Stack<BoundScope> _scopes = new([BoundScope]);
    private readonly Stack<LoopScope> _loops = [];
    private readonly Stack<LambdaTypeSymbol> _lambdas = [];

    private int _labelId = 0;

    public BoundScope BoundScope { get => _scopes.Peek(); }

    public LoopScope? LoopScope { get => _loops.TryPeek(out var loopScope) ? loopScope : null; }

    public LambdaTypeSymbol? Lambda { get => _lambdas.TryPeek(out var lambda) ? lambda : null; }

    public DiagnosticBag Diagnostics { get => BoundTree.Diagnostics; }

    public IDisposable PushBoundScope() => Disposable.BoundScope(this, new BoundScope(BoundScope));
    public IDisposable PushLoopScope()
    {
        var labelId = Interlocked.Increment(ref _labelId);
        var continueLabel = new LabelSymbol($"continue<{labelId}>");
        var breakLabel = new LabelSymbol($"break<{labelId}>");
        return Disposable.LoopScope(this, new LoopScope(continueLabel, breakLabel));
    }
    public IDisposable PushLambdaScope(LambdaTypeSymbol lambda) => Disposable.LambdaScope(this, lambda);

    private readonly struct Disposable : IDisposable
    {
        private readonly Action _pop;
        private Disposable(Action push, Action pop)
        {
            push();
            _pop = pop;
        }

        public static Disposable BoundScope(BinderContext context, BoundScope boundScope) => new(
            () => context._scopes.Push(boundScope),
            () => context._scopes.Pop());

        public static Disposable LoopScope(BinderContext context, LoopScope loopScope) => new(
            () => context._loops.Push(loopScope),
            () => context._loops.Pop());

        public static Disposable LambdaScope(BinderContext context, LambdaTypeSymbol lambdaType) => new(
            () => context._lambdas.Push(lambdaType),
            () => context._lambdas.Pop());

        public void Dispose() => _pop();
    }
}
