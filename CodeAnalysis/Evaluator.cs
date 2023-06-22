using CodeAnalysis.Binding;
using CodeAnalysis.Symbols;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeAnalysis;

internal sealed class Evaluator : IBoundExpressionVisitor<object?>, IBoundStatementVisitor
{
    private readonly BoundStatement _boundStatement;
    private readonly Dictionary<VariableSymbol, object?> _variables;

    private object? _lastValue;

    public Evaluator(BoundStatement boundStatement, Dictionary<VariableSymbol, object?> variables)
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
        var condition = (bool)EvaluateExpression(statement.Condition)!;
        if (condition)
            EvaluateStatement(statement.Then);
        else if (statement.HasElseClause)
            EvaluateStatement(statement.Else);
    }

    void IBoundStatementVisitor.Accept(BoundWhileStatement statement)
    {
        while ((bool)EvaluateExpression(statement.Condition)!)
            EvaluateStatement(statement.Body);
    }

    void IBoundStatementVisitor.Accept(BoundForStatement statement)
    {
        var lowerBound = (int)EvaluateExpression(statement.LowerBound)!;
        var upperBound = (int)EvaluateExpression(statement.UpperBound)!;

        for (var i = lowerBound; i < upperBound; ++i)
        {
            _variables[statement.Variable] = i;
            EvaluateStatement(statement.Body);
        }
    }

    void IBoundStatementVisitor.Accept(BoundDeclarationStatement statement) => _lastValue = _variables[statement.Variable] = EvaluateExpression(statement.Expression);

    void IBoundStatementVisitor.Accept(BoundExpressionStatement statement) => _lastValue = EvaluateExpression(statement.Expression);

    private object? EvaluateExpression(BoundExpression expression) => expression.Accept(this);

    object? IBoundExpressionVisitor<object?>.Visit(BoundNeverExpression expression) => _lastValue;

    object? IBoundExpressionVisitor<object?>.Visit(BoundIfExpression expression)
    {
        var isTrue = (bool)EvaluateExpression(expression.Condition)!;

        return isTrue ? EvaluateExpression(expression.Then) : EvaluateExpression(expression.Else);
    }

    object? IBoundExpressionVisitor<object?>.Visit(BoundLiteralExpression expression) => expression.Value!;

    object? IBoundExpressionVisitor<object?>.Visit(BoundVariableExpression expression) => _variables[expression.Variable];

    object? IBoundExpressionVisitor<object?>.Visit(BoundCallExpression expression)
    {
        switch (expression.Function.Name)
        {
            case string name when name == BuiltinFunctions.ReadLine.Name:
                return Console.ReadLine();

            case string name when name == BuiltinFunctions.WriteLine.Name:
                Console.WriteLine(EvaluateExpression(expression.Arguments[0]));
                return null;

            case string name when name == BuiltinFunctions.ToStr.Name:
                return EvaluateExpression(expression.Arguments[0])?.ToString();

            case string name when name == BuiltinFunctions.IsSame.Name:
                return ReferenceEquals(EvaluateExpression(expression.Arguments[0]), EvaluateExpression(expression.Arguments[1]));

            case string name when name == BuiltinFunctions.Random.Name:
                return Random.Shared.Next((int)EvaluateExpression(expression.Arguments[0])!);

            default:
                throw new InvalidOperationException($"Undefined function {expression.Function.Name}");
        }
    }

    object? IBoundExpressionVisitor<object?>.Visit(BoundConvertExpression expression) => expression.Type.Convert(EvaluateExpression(expression.Expression));

    object? IBoundExpressionVisitor<object?>.Visit(BoundAssignmentExpression expression) => _variables[expression.Variable] = expression.Expression.Accept(this);

    object? IBoundExpressionVisitor<object?>.Visit(BoundUnaryExpression expression)
    {
        var operation = expression.GetOperation();
        var value = expression.Operand.Accept(this);

        return operation.Invoke(value);
    }

    object? IBoundExpressionVisitor<object?>.Visit(BoundBinaryExpression expression)
    {
        var operation = expression.GetOperation();
        var left = expression.Left.Accept(this);
        var right = expression.Right.Accept(this);

        return operation.Invoke(left, right);
    }
}

file static class BoundExpressionExtensions
{
    private readonly record struct UnaryExpressionCacheKey(BoundUnaryOperatorKind Kind, TypeSymbol OperandType);
    private readonly static ConcurrentDictionary<BoundUnaryOperatorKind, ConcurrentDictionary<UnaryExpressionCacheKey, Func<object?, object?>>> UnaryExpressionCache = new();
    private readonly static ConcurrentDictionary<BoundUnaryOperatorKind, Func<Expression, UnaryExpression>> UnaryOperatorCache = new();

    private readonly record struct BinaryExpressionCacheKey(BoundBinaryOperatorKind Kind, TypeSymbol LeftType, TypeSymbol RightType);
    private readonly static ConcurrentDictionary<BoundBinaryOperatorKind, ConcurrentDictionary<BinaryExpressionCacheKey, Func<object?, object?, object?>>> BinaryExpressionCache = new();
    private readonly static ConcurrentDictionary<BoundBinaryOperatorKind, Func<Expression, Expression, BinaryExpression>> BinaryOperatorCache = new();

    public static Func<Expression, UnaryExpression> GetExpressionFactory(this BoundUnaryOperatorKind kind)
    {
        return UnaryOperatorCache.GetOrAdd(kind, static kind =>
        {
            var operand = Expression.Parameter(typeof(Expression), "operand");
            var method = typeof(Expression).GetMethod(kind.ToString(), BindingFlags.Public | BindingFlags.Static, new[] { typeof(Expression) });
            if (method is null)
                throw new InvalidOperationException($"Unexpected binary operator {kind}");

            var body = Expression.Call(
                instance: null,
                method,
                operand);

            var func = Expression.Lambda<Func<Expression, UnaryExpression>>(body, operand);
            return func.Compile();
        });
    }

    public static Func<Expression, Expression, BinaryExpression> GetExpressionFactory(this BoundBinaryOperatorKind kind)
    {
        return BinaryOperatorCache.GetOrAdd(kind, static kind =>
        {
            var left = Expression.Parameter(typeof(Expression), "left");
            var right = Expression.Parameter(typeof(Expression), "right");
            var method = typeof(Expression).GetMethod(kind.ToString(), BindingFlags.Public | BindingFlags.Static, new[] { typeof(Expression), typeof(Expression) });
            if (method is null)
                throw new InvalidOperationException($"Unexpected binary operator {kind}");

            var body = Expression.Call(
                instance: null,
                method,
                left,
                right);

            var func = Expression.Lambda<Func<Expression, Expression, BinaryExpression>>(body, left, right);
            return func.Compile();
        });
    }

    public static Func<object?, object?> GetOperation(this BoundUnaryExpression expression)
    {
        var cacheNode = UnaryExpressionCache.GetOrAdd(expression.Operator.Kind, kind => new ConcurrentDictionary<UnaryExpressionCacheKey, Func<object?, object?>>());

        return cacheNode.GetOrAdd(new UnaryExpressionCacheKey(expression.Operator.Kind, expression.Operand.Type), CreateOperation);

        static Func<object?, object?> CreateOperation(UnaryExpressionCacheKey key)
        {
            var (kind, operandType) = key;
            var operandClrType = operandType.GetClrType();

            var operand = Expression.Parameter(typeof(object), "operand");
            var expr = kind.GetExpressionFactory();
            var body = Expression.Convert(expr.Invoke(Expression.Convert(operand, operandClrType)), typeof(object));
            var expression = Expression.Lambda<Func<object?, object?>>(body, operand);

            return expression.Compile();
        }
    }

    public static Func<object?, object?, object?> GetOperation(this BoundBinaryExpression expression)
    {
        var cacheNode = BinaryExpressionCache.GetOrAdd(expression.Operator.Kind, kind => new ConcurrentDictionary<BinaryExpressionCacheKey, Func<object?, object?, object?>>());

        return cacheNode.GetOrAdd(new BinaryExpressionCacheKey(expression.Operator.Kind, expression.Left.Type, expression.Right.Type), CreateOperation);

        static Func<object?, object?, object?> CreateOperation(BinaryExpressionCacheKey key)
        {
            var (kind, leftType, rightType) = key;
            var leftClrType = leftType.GetClrType();
            var rightClrType = rightType.GetClrType();
            Expression<Func<object?, object?, object?>> expression;
            if (kind == BoundBinaryOperatorKind.Add && (leftClrType == typeof(string) || rightClrType == typeof(string)))
            {
                var left = Expression.Parameter(typeof(object), "left");
                var right = Expression.Parameter(typeof(object), "right");

                var method = typeof(string).GetMethod(nameof(String.Concat), new[] { typeof(object), typeof(object) });
                if (method is null)
                    throw new InvalidOperationException($"Could not find {nameof(String.Concat)} method");

                var body = Expression.Convert(Expression.Call(instance: null, method, left, right), typeof(object));
                expression = Expression.Lambda<Func<object?, object?, object?>>(body, left, right);
            }
            else
            {
                var left = Expression.Parameter(typeof(object), "left");
                var right = Expression.Parameter(typeof(object), "right");
                var expr = kind.GetExpressionFactory();
                var body = Expression.Convert(expr.Invoke(Expression.Convert(left, leftClrType), Expression.Convert(right, rightClrType)), typeof(object));
                expression = Expression.Lambda<Func<object?, object?, object?>>(body, left, right);
            }
            return expression.Compile();
        }
    }
}
