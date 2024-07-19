using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class FunctionValue : PrimValue
{
    public FunctionValue(FunctionType functionType, Delegate @delegate) : base(functionType)
    {
        FunctionType = functionType;
        Delegate = @delegate;
        Set(
            MethodSymbol.FromOperator(functionType.GetOperators(SyntaxKind.ParenthesisOpenParenthesisCloseToken).Single()),
            this);
    }

    public FunctionType FunctionType { get; init; }
    public Delegate Delegate { get; init; }

    public override Delegate Value => Delegate;

    public PrimValue Invoke(params ReadOnlySpan<PrimValue> arguments) =>
        (PrimValue)Delegate.DynamicInvoke(arguments.ToArray())!;
}
