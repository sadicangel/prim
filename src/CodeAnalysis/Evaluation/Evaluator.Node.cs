using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Evaluation.Values;

namespace CodeAnalysis.Evaluation;
partial class Evaluator
{
    private static PrimValue EvaluateNode(BoundNode node, EvaluatorContext context)
    {
        return node switch
        {
            BoundExpression expression => EvaluateExpression(expression, context),
            _ => throw new UnreachableException($"Unexpected node type '{node.GetType().Name}'")
        };
    }
}
