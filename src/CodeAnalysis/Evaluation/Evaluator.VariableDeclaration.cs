using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Evaluation.Values;

namespace CodeAnalysis.Evaluation;
partial class Evaluator
{
    private static LiteralValue EvaluateVariableDeclaration(BoundVariableDeclaration node, EvaluatorContext context)
    {
        var value = EvaluateExpression(node.Expression, context);
        context.EvaluatedScope.Declare(node.VariableSymbol, value);
        return LiteralValue.Unit;
    }
}
