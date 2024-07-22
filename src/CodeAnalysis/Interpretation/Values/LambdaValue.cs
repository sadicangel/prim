using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class LambdaValue : PrimValue
{
    public LambdaValue(LambdaTypeSymbol lambdaType, Delegate @delegate) : base(lambdaType)
    {
        LambdaType = lambdaType;
        Delegate = @delegate;
        var invocationOperator = lambdaType.GetOperators(SyntaxKind.ParenthesisOpenParenthesisCloseToken).Single();
        Set(invocationOperator, this);
    }

    public LambdaTypeSymbol LambdaType { get; init; }
    public Delegate Delegate { get; init; }

    public override Delegate Value => Delegate;

    public bool Equals(LambdaValue? other) => LambdaType == other?.LambdaType && ReferenceEquals(Delegate, other.Delegate);
    public override int GetHashCode() => HashCode.Combine(LambdaType, Delegate);

    public PrimValue Invoke(params ReadOnlySpan<PrimValue> arguments) =>
        (PrimValue)Delegate.DynamicInvoke(arguments.ToArray())!;
}
