using CodeAnalysis.Binding;

namespace CodeAnalysis;

public sealed class Evaluator : IBoundExpressionVisitor<object>
{
    private readonly BoundExpression _boundExpression;

    public Evaluator(BoundExpression boundExpression)
    {
        _boundExpression = boundExpression;
    }

    public object Evaluate() => _boundExpression.Accept(this);


    object IBoundExpressionVisitor<object>.Visit(BoundExpression expression) => expression.Accept(this);

    object IBoundExpressionVisitor<object>.Visit(BoundUnaryExpression expression)
    {
        var operation = GetOperation(expression.Operator.Kind);
        var value = expression.Operand.Accept(this);

        return operation.Invoke(value);

        static Func<object, object> GetOperation(BoundUnaryOperatorKind kind) => kind switch
        {
            BoundUnaryOperatorKind.Identity => value => value,
            BoundUnaryOperatorKind.Negation => value => -(long)value,
            BoundUnaryOperatorKind.LogicalNegation => value => !(bool)value,
            _ => throw new InvalidOperationException($"Unexpected unary operator {kind}"),
        };
    }

    object IBoundExpressionVisitor<object>.Visit(BoundBinaryExpression expression)
    {
        var operation = GetOperation(expression.Operator.Kind);
        var left = expression.Left.Accept(this);
        var right = expression.Right.Accept(this);

        return operation.Invoke(left, right);

        static Func<object, object, object> GetOperation(BoundBinaryOperatorKind kind) => kind switch
        {
            BoundBinaryOperatorKind.Addition => static (l, r) => (long)l + (long)r,
            BoundBinaryOperatorKind.Subtraction => static (l, r) => (long)l - (long)r,
            BoundBinaryOperatorKind.Multiplication => static (l, r) => (long)l * (long)r,
            BoundBinaryOperatorKind.Division => static (l, r) => (long)l / (long)r,
            BoundBinaryOperatorKind.LogicalAnd => static (l, r) => (bool)l && (bool)r,
            BoundBinaryOperatorKind.LogicalOr => static (l, r) => (bool)l || (bool)r,
            BoundBinaryOperatorKind.Equals => static (l, r) => Equals(l, r),
            BoundBinaryOperatorKind.NotEquals => static (l, r) => !Equals(l, r),
            _ => throw new InvalidOperationException($"Unexpected binary operator {kind}"),
        };
    }

    object IBoundExpressionVisitor<object>.Visit(BoundLiteralExpression expression) => expression.Value!;
}
