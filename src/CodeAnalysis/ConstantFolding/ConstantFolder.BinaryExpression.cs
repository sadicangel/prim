using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.ConstantFolding;
partial class ConstantFolder
{
    private static object? FoldBinaryExpression(BoundBinaryExpression node)
    {
        var left = node.Left.ConstantValue;
        var right = node.Right.ConstantValue;

        if (left is null || right is null)
        {
            return null;
        }

        var typedValue = Interpreter.EvaluateBinaryExpression(
            node,
            new Interpreter.Context(ModuleValue.CreateGlobalModule(node.Type.ContainingModule), []));

        Debug.Assert(typedValue.Type == node.Type);

        return typedValue.Value;
    }
}
