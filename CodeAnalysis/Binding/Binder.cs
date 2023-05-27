using CodeAnalysis.Syntax;
using System.Runtime.Serialization;

namespace CodeAnalysis.Binding;

public sealed class Binder : IExpressionVisitor<BoundExpression>
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly Dictionary<Variable, object> _variables;

    public Binder(Dictionary<Variable, object> variables)
    {
        _variables = variables;
    }

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
            _diagnostics.ReportUndefinedUnaryOperator(expression.OperatorToken, boundOperand.Type);
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
            _diagnostics.ReportUndefinedBinaryOperator(expression.OperatorToken, boundLeft.Type, boundRight.Type);
            return boundLeft;
        }
        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(LiteralExpression expression)
    {
        var value = expression.Value ?? 0L;
        return new BoundLiteralExpression(value);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(NameExpression expression)
    {
        var variable = _variables.Keys.FirstOrDefault(v => v.Name == expression.IdentifierToken.Text);
        if (variable is null)
        {
            _diagnostics.ReportUndefinedName(expression.IdentifierToken);
            return new BoundLiteralExpression(0);
        }

        return new BoundVariableExpression(variable);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(AssignmentExpression expression)
    {
        var name = expression.IdentifierToken.Text;
        var boundExpression = BindExpression(expression.Expression);

        var existingVariable = _variables.Keys.FirstOrDefault(v => v.Name == name);
        if (existingVariable is not null)
            _variables.Remove(existingVariable);

        var variable = new Variable(name, boundExpression.Type);
        _variables[variable] = boundExpression.Type.IsValueType
            ? FormatterServices.GetUninitializedObject(boundExpression.Type)
            : throw new InvalidOperationException($"Unsupported variable type {boundExpression.Type.Name}");

        return new BoundAssignmentExpression(variable, boundExpression);
    }
}