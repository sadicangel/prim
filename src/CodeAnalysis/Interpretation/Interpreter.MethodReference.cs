using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ReferenceValue EvaluateMethodReference(BoundMethodReference node, Context context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        context.EvaluatedScope.Declare(VariableSymbol.This(expression.Type), expression);
        var methodReference = new ReferenceValue(
            node.Symbol.Type,
            () => expression.Get(node.Symbol),
            pv => expression.Set(node.Symbol, pv));
        return methodReference;
    }
}
