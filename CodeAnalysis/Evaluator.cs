using CodeAnalysis.Binding;

namespace CodeAnalysis;

internal sealed class Evaluator : IBoundExpressionVisitor<object>, IBoundStatementVisitor
{
    private readonly BoundStatement _boundStatement;
    private readonly Dictionary<Variable, object> _variables;

    private object? _lastValue;

    public Evaluator(BoundStatement boundStatement, Dictionary<Variable, object> variables)
    {
        _boundStatement = boundStatement;
        _variables = variables;
    }

    public object? Evaluate()
    {
        EvaluateStatement(_boundStatement);
        return _lastValue;
    }

    private void EvaluateStatement(BoundStatement statement) => statement.Accept(this);

    void IBoundStatementVisitor.Accept(BoundBlockStatement statement)
    {
        foreach (var s in statement.Statements)
            EvaluateStatement(s);
    }
    void IBoundStatementVisitor.Accept(BoundIfStatement statement)
    {
        var condition = (bool)EvaluateExpression(statement.Condition);
        if (condition)
            EvaluateStatement(statement.Then);
        else if (statement.HasElseClause)
            EvaluateStatement(statement.Else);
    }

    void IBoundStatementVisitor.Accept(BoundWhileStatement statement)
    {
        while ((bool)EvaluateExpression(statement.Condition))
            EvaluateStatement(statement.Body);
    }

    void IBoundStatementVisitor.Accept(BoundForStatement statement)
    {
        var lowerBound = (int)EvaluateExpression(statement.LowerBound);
        var upperBound = (int)EvaluateExpression(statement.UpperBound);

        for (var i = lowerBound; i <= upperBound; ++i)
        {
            _variables[statement.Variable] = i;
            EvaluateStatement(statement.Body);
        }
    }

    void IBoundStatementVisitor.Accept(BoundDeclarationStatement statement) => _lastValue = _variables[statement.Variable] = EvaluateExpression(statement.Expression);

    void IBoundStatementVisitor.Accept(BoundExpressionStatement statement) => _lastValue = EvaluateExpression(statement.Expression);

    private object EvaluateExpression(BoundExpression expression) => expression.Accept(this);

    object IBoundExpressionVisitor<object>.Visit(BoundIfExpression expression)
    {
        var isTrue = (bool)EvaluateExpression(expression.Condition);

        return isTrue ? EvaluateExpression(expression.Then) : EvaluateExpression(expression.Else);
    }

    object IBoundExpressionVisitor<object>.Visit(BoundLiteralExpression expression) => expression.Value!;

    object IBoundExpressionVisitor<object>.Visit(BoundVariableExpression expression) => _variables[expression.Variable];

    object IBoundExpressionVisitor<object>.Visit(BoundAssignmentExpression expression) => _variables[expression.Variable] = expression.Expression.Accept(this);

    object IBoundExpressionVisitor<object>.Visit(BoundUnaryExpression expression)
    {
        var operation = GetOperation(expression.Operator.Kind);
        var value = expression.Operand.Accept(this);

        return operation.Invoke(value);

        static Func<object, object> GetOperation(BoundUnaryOperatorKind kind) => kind switch
        {
            BoundUnaryOperatorKind.Identity => value => value,
            BoundUnaryOperatorKind.Negation => value => -(int)value,
            BoundUnaryOperatorKind.LogicalNegation => value => !(bool)value,
            BoundUnaryOperatorKind.OnesComplement => value => ~(int)value,
            _ => throw new InvalidOperationException($"Unexpected unary operator {kind}"),
        };
    }

    object IBoundExpressionVisitor<object>.Visit(BoundBinaryExpression expression)
    {
        var operation = GetOperation(expression.Operator.Kind, expression.Left.Type, expression.Right.Type);
        var left = expression.Left.Accept(this);
        var right = expression.Right.Accept(this);

        return operation.Invoke(left, right);

        static Func<object, object, object> GetOperation(BoundBinaryOperatorKind kind, Type leftType, Type rightType) => kind switch
        {
            BoundBinaryOperatorKind.Addition => static (l, r) => (int)l + (int)r,
            BoundBinaryOperatorKind.And when leftType == typeof(bool) => static (l, r) => (bool)l & (bool)r,
            BoundBinaryOperatorKind.And when leftType == typeof(int) => static (l, r) => (int)l & (int)r,
            BoundBinaryOperatorKind.AndAlso => static (l, r) => (bool)l && (bool)r,
            BoundBinaryOperatorKind.Division => static (l, r) => (int)l / (int)r,
            BoundBinaryOperatorKind.Equals => static (l, r) => Equals(l, r),
            BoundBinaryOperatorKind.ExclusiveOr when leftType == typeof(bool) => static (l, r) => (bool)l ^ (bool)r,
            BoundBinaryOperatorKind.ExclusiveOr when leftType == typeof(int) => static (l, r) => (int)l ^ (int)r,
            BoundBinaryOperatorKind.GreaterThan => static (l, r) => (int)l > (int)r,
            BoundBinaryOperatorKind.GreaterThanOrEqualTo => static (l, r) => (int)l >= (int)r,
            BoundBinaryOperatorKind.LessThan => static (l, r) => (int)l < (int)r,
            BoundBinaryOperatorKind.LessThanOrEqualTo => static (l, r) => (int)l <= (int)r,
            BoundBinaryOperatorKind.Modulo => static (l, r) => (int)l % (int)r,
            BoundBinaryOperatorKind.Multiplication => static (l, r) => (int)l * (int)r,
            BoundBinaryOperatorKind.NotEquals => static (l, r) => !Equals(l, r),
            BoundBinaryOperatorKind.Or when leftType == typeof(bool) => static (l, r) => (bool)l | (bool)r,
            BoundBinaryOperatorKind.Or when leftType == typeof(int) => static (l, r) => (int)l | (int)r,
            BoundBinaryOperatorKind.OrElse => static (l, r) => (bool)l || (bool)r,
            BoundBinaryOperatorKind.Subtraction => static (l, r) => (int)l - (int)r,
            _ => throw new InvalidOperationException($"Unexpected binary operator {kind}"),
        };
    }
}
