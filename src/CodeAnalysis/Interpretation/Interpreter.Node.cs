using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateNode(BoundNode node, InterpreterContext context)
    {
        return node switch
        {
            BoundExpression expression => EvaluateExpression(expression, context),
            _ => throw new UnreachableException($"Unexpected node type '{node.GetType().Name}'")
        };
    }
}
