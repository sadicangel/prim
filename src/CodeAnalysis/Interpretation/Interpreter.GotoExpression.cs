using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateGotoExpression(BoundGotoExpression node, InterpreterContext context)
    {
        var value = EvaluateExpression(node.Expression, context);
        context.InstructionIndex = context.LabelIndices[node.LabelSymbol];
        return value;
    }
}
