using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.ConstFolding;
partial class ConstFolder
{
    private static object? FoldIfElseExpression(BoundIfElseExpression node)
    {
        if (node.Condition.ConstValue is not bool isTrue)
        {
            return null;
        }

        var constValue = isTrue ? node.Then.ConstValue : node.Else.ConstValue;
        return constValue;
    }
}
