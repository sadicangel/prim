using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.ConstantFolding;
partial class ConstantFolder
{
    private static object? FoldIfElseExpression(BoundIfExpression node)
    {
        if (node.Condition.ConstantValue is not bool isTrue)
        {
            return null;
        }

        var constValue = isTrue ? node.Then.ConstantValue : node.Else.ConstantValue;
        return constValue;
    }
}
