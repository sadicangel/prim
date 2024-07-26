using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundExpression LowerConditionalGotoExpression(BoundConditionalGotoExpression node, LowererContext context)
    {
        if (node.Condition.ConstantValue is bool constantValue)
        {
            var shouldJump = node.JumpTrue ? constantValue : !constantValue;

            var unconditional = shouldJump
                ? new BoundGotoExpression(node.Syntax, node.LabelSymbol, node.Expression)
                : new BoundNopExpression(node.Syntax) as BoundExpression;

            return LowerExpression(unconditional, context);
        }

        if (node.Condition.ConstantValue is not null)
            throw new UnreachableException($"Unexpected constant value '{node.Condition.ConstantValue}' for node '{node.BoundKind}'");

        var condition = LowerExpression(node.Condition, context);
        var expression = LowerExpression(node.Expression, context);
        if (ReferenceEquals(condition, node.Condition) && ReferenceEquals(expression, node.Expression))
            return node;

        return node with { Expression = expression };
    }
}
