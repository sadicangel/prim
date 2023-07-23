using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Binding;

internal static class ConstantFolding
{
    public static ConstantValue? Fold(BoundUnaryOperator @operator, BoundExpression operand)
    {
        if (operand.ConstantValue is null)
            return null;

        var operandValue = operand.ConstantValue.Value;

        var operation = @operator.GetOperation(operand);
        var value = operation.Invoke(operandValue);
        return new ConstantValue(value);
    }

    public static ConstantValue? Fold(BoundExpression left, BoundBinaryOperator @operator, BoundExpression right)
    {
        var leftConstant = left.ConstantValue;
        var leftValue = leftConstant?.Value;

        var rightConstant = right.ConstantValue;
        var rightValue = rightConstant?.Value;

        switch (@operator.Kind)
        {
            // Short circuit and.
            case BoundBinaryOperatorKind.AndAlso when leftValue is false || rightValue is false:
                return new ConstantValue(false);

            // Short circuit else.
            case BoundBinaryOperatorKind.OrElse when leftValue is true || rightValue is true:
                return new ConstantValue(true);

            case BoundBinaryOperatorKind _ when leftConstant is not null && rightConstant is not null:
                var operation = @operator.GetOperation(left, right);
                var value = operation.Invoke(leftValue, rightValue);
                return new ConstantValue(value);

            default:
                return null;
        }
    }
}
