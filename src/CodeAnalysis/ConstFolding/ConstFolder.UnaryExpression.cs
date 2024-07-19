using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation;

namespace CodeAnalysis.ConstFolding;
partial class ConstFolder
{
    private static object? FoldUnaryExpression(BoundUnaryExpression node)
    {
        var operand = node.Operand.ConstValue;
        if (operand is null)
        {
            return null;
        }

        var typedValue = Interpreter.EvaluateUnaryExpression(node, new InterpreterContext(GlobalEvaluatedScope.Instance));

        Debug.Assert(typedValue.Type == node.Type);

        return typedValue.Value;
    }
}

