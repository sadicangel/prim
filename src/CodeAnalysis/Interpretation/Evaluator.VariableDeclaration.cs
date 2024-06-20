using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Evaluator
{
    private static LiteralValue EvaluateVariableDeclaration(BoundVariableDeclaration node, EvaluatorContext context)
    {
        var value = EvaluateExpression(node.Expression, context);
        context.EvaluatedScope.Declare(node.VariableSymbol, value);
        return LiteralValue.Unit;
    }
}
