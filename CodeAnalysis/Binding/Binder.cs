using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

public sealed class Binder : IExpressionVisitor<BoundExpression>
{
    private readonly List<Diagnostic> _diagnostics = new();

    public BoundExpression BindExpression(Expression expression)
    {
        IExpressionVisitor<BoundExpression> visitor = this;
        return visitor.Visit(expression);
    }

    public IEnumerable<Diagnostic> Diagnostics { get => _diagnostics; }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(Expression expression) => expression.Accept(this);

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(GroupExpression expression) => expression.Expression.Accept(this);

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(UnaryExpression expression)
    {
        var boundOperand = BindExpression(expression.Operand);
        var boundOperator = BoundUnaryOperator.Bind(expression.OperatorToken.Kind, boundOperand.Type);
        if (boundOperator is null)
        {
            _diagnostics.Add(Diagnostic.InvalidUnaryOperator(expression.OperatorToken.Text, boundOperand.Type));
            return boundOperand;
        }
        return new BoundUnaryExpression(boundOperator, boundOperand);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(BinaryExpression expression)
    {
        var boundLeft = BindExpression(expression.Left);
        var boundRight = BindExpression(expression.Right);
        var boundOperator = BoundBinaryOperator.Bind(expression.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
        if (boundOperator is null)
        {
            _diagnostics.Add(Diagnostic.InvalidBinaryOperator(expression.OperatorToken.Text, boundLeft.Type, boundRight.Type));
            return boundLeft;
        }
        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(LiteralExpression expression)
    {
        var value = expression.Value ?? 0L;
        return new BoundLiteralExpression(value);
    }
}