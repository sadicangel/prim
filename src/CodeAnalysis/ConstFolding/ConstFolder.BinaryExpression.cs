using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation;

namespace CodeAnalysis.ConstFolding;
partial class ConstFolder
{
    private static object? FoldBinaryExpression(BoundBinaryExpression node)
    {
        var left = node.Left.ConstValue;
        var right = node.Right.ConstValue;

        if (left is null || right is null)
        {
            return null;
        }

        var typedValue = Interpreter.EvaluateBinaryExpression(
            node,
            new InterpreterContext(GlobalEvaluatedScope.Instance));

        Debug.Assert(typedValue.Type == node.Type);

        return typedValue.Value;
    }
}
