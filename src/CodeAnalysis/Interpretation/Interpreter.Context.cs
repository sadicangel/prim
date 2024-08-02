﻿using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;

partial class Interpreter
{
    public sealed record class Context(EvaluatedScope EvaluatedScope, Dictionary<LabelSymbol, int> LabelIndices)
    {
        private readonly Stack<EvaluatedScope> _scopes = new([EvaluatedScope]);

        public EvaluatedScope EvaluatedScope { get => _scopes.Peek(); }
        public int InstructionIndex { get; set; }
        public PrimValue LastValue { get; set; } = new InstanceValue(EvaluatedScope.Unit, CodeAnalysis.Unit.Value);

        internal InstanceValue Unit { get; } = new InstanceValue(EvaluatedScope.Unit, CodeAnalysis.Unit.Value);
        internal InstanceValue True { get; } = new InstanceValue(EvaluatedScope.Bool, true);
        internal InstanceValue False { get; } = new InstanceValue(EvaluatedScope.Bool, false);
        internal InstanceValue EmptyStr { get; } = new InstanceValue(EvaluatedScope.Str, string.Empty);

        public IDisposable PushScope() => Disposable.EvaluatedScope(this, new EvaluatedScope(EvaluatedScope));

        private readonly struct Disposable : IDisposable
        {
            private readonly Action _pop;
            private Disposable(Action push, Action pop)
            {
                push();
                _pop = pop;
            }

            public static Disposable EvaluatedScope(Context context, EvaluatedScope evaluatedScope) => new(
                () => context._scopes.Push(evaluatedScope),
                () => context._scopes.Pop());

            public void Dispose() => _pop();
        }
    }
}
