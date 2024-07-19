namespace CodeAnalysis.Interpretation;

internal readonly record struct InterpreterContext(EvaluatedScope EvaluatedScope)
{
    private readonly Stack<EvaluatedScope> _scopes = new([EvaluatedScope]);
    public EvaluatedScope EvaluatedScope { get => _scopes.Peek(); }

    public TempScope PushScope() => new(this);

    internal readonly ref struct TempScope
    {
        private readonly InterpreterContext _parent;
        public TempScope(InterpreterContext parent)
        {
            _parent = parent;
            _parent._scopes.Push(new EvaluatedScope(_parent.EvaluatedScope));
        }
        public void Dispose() => _parent._scopes?.Pop();
    }
}
