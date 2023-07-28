using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Symbols;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeAnalysis.Binding.Expressions;

internal static class BoundExpressionExtensions
{
    private readonly record struct UnaryExpressionCacheKey(BoundUnaryOperatorKind Kind, TypeSymbol OperandType);
    private readonly static ConcurrentDictionary<BoundUnaryOperatorKind, ConcurrentDictionary<UnaryExpressionCacheKey, Func<object?, object?>>> UnaryExpressionCache = new();
    private readonly static ConcurrentDictionary<BoundUnaryOperatorKind, Func<Expression, UnaryExpression>> UnaryOperatorCache = new();

    private readonly record struct BinaryExpressionCacheKey(BoundBinaryOperatorKind Kind, TypeSymbol LeftType, TypeSymbol RightType, TypeSymbol ResultType);
    private readonly static ConcurrentDictionary<BoundBinaryOperatorKind, ConcurrentDictionary<BinaryExpressionCacheKey, Func<object?, object?, object?>>> BinaryExpressionCache = new();
    private readonly static ConcurrentDictionary<BoundBinaryOperatorKind, Func<Expression, Expression, BinaryExpression>> BinaryOperatorCache = new();

    private static Func<Expression, UnaryExpression> GetExpressionFactory(this BoundUnaryOperatorKind kind)
    {
        return UnaryOperatorCache.GetOrAdd(kind, static kind =>
        {
            var operand = Expression.Parameter(typeof(Expression), "operand");
            var method = typeof(Expression).GetMethod(kind.GetLinqExpressionName(), BindingFlags.Public | BindingFlags.Static, new[] { typeof(Expression) });
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

    public static Func<object?, object?> GetOperation(this BoundUnaryOperator @operator, BoundExpression operand)
    {
        var cacheNode = UnaryExpressionCache.GetOrAdd(@operator.Kind, kind => new ConcurrentDictionary<UnaryExpressionCacheKey, Func<object?, object?>>());

        return cacheNode.GetOrAdd(new UnaryExpressionCacheKey(@operator.Kind, operand.Type), CreateOperation);

        static Func<object?, object?> CreateOperation(UnaryExpressionCacheKey key)
        {
            var (kind, operandType) = key;

            var operand = Expression.Parameter(typeof(object), "operand");
            var expr = kind.GetExpressionFactory();
            var body = Expression.Convert(expr.Invoke(Expression.Convert(operand, operandType.ClrType)), typeof(object));
            var expression = Expression.Lambda<Func<object?, object?>>(body, operand);

            return expression.Compile();
        }
    }

    public static Func<object?, object?> GetOperation(this BoundUnaryExpression expression) => GetOperation(expression.Operator, expression.Operand);

    private static Func<Expression, Expression, BinaryExpression> GetExpressionFactory(this BoundBinaryOperatorKind kind)
    {
        return BinaryOperatorCache.GetOrAdd(kind, static kind =>
        {
            var leftParam = Expression.Parameter(typeof(Expression), "left");
            var rightParam = Expression.Parameter(typeof(Expression), "right");
            var method = typeof(Expression).GetMethod(kind.GetLinqExpressionName(), BindingFlags.Public | BindingFlags.Static, new[] { typeof(Expression), typeof(Expression) });
            if (method is null)
                throw new InvalidOperationException($"Unexpected binary operator {kind}");

            var body = Expression.Call(instance: null, method, leftParam, rightParam);

            var func = Expression.Lambda<Func<Expression, Expression, BinaryExpression>>(body, leftParam, rightParam);
            return func.Compile();
        });
    }

    public static Func<object?, object?, object?> GetOperation(this BoundBinaryOperator @operator, BoundExpression left, BoundExpression right)
    {
        var cacheNode = BinaryExpressionCache.GetOrAdd(@operator.Kind, kind => new ConcurrentDictionary<BinaryExpressionCacheKey, Func<object?, object?, object?>>());

        var rightType = right.Type;
        if (@operator.Kind is BoundBinaryOperatorKind.ImplicitCast or BoundBinaryOperatorKind.ExplicitCast)
        {
            var symbolExpression = (BoundSymbolExpression)right;
            if (!PredefinedTypes.TryLookup(symbolExpression.Symbol.Name, out rightType))
                throw new InvalidOperationException($"Could not find type {symbolExpression.Symbol.Name}");
        }

        return cacheNode.GetOrAdd(new BinaryExpressionCacheKey(@operator.Kind, left.Type, rightType, @operator.ResultType), CreateOperation);

        static Func<object?, object?, object?> CreateOperation(BinaryExpressionCacheKey key)
        {
            var (kind, leftType, rightType, resultType) = key;

            var leftParam = Expression.Parameter(typeof(object), "left");
            var rightParam = Expression.Parameter(typeof(object), "right");

            Expression<Func<object?, object?, object?>> expression;
            switch (kind)
            {
                case BoundBinaryOperatorKind.Add when leftType == PredefinedTypes.Str || rightType == PredefinedTypes.Str:
                    {
                        var method = typeof(string).GetMethod(nameof(String.Concat), new[] { typeof(object), typeof(object) });
                        if (method is null)
                            throw new InvalidOperationException($"Could not find {nameof(String.Concat)} method");

                        var body = Expression.Convert(Expression.Call(instance: null, method, leftParam, rightParam), typeof(object));
                        expression = Expression.Lambda<Func<object?, object?, object?>>(body, leftParam, rightParam);
                    }
                    break;

                case BoundBinaryOperatorKind.Exponent:
                    {
                        var method = typeof(Math).GetMethod(nameof(Math.Pow), new[] { typeof(double), typeof(double) });
                        if (method is null)
                            throw new InvalidOperationException($"Could not find {nameof(Math.Pow)} method");

                        var leftConv = Expression.Convert(Expression.Convert(leftParam, leftType.ClrType), typeof(double));
                        var rightConv = Expression.Convert(Expression.Convert(rightParam, rightType.ClrType), typeof(double));

                        var body = Expression.Convert(Expression.Convert(Expression.Call(instance: null, method, leftConv, rightConv), leftType.ClrType), typeof(object));
                        expression = Expression.Lambda<Func<object?, object?, object?>>(body, leftParam, rightParam);
                    }
                    break;

                case BoundBinaryOperatorKind.ExplicitCast or BoundBinaryOperatorKind.ImplicitCast:
                    {
                        var body = Expression.Convert(Expression.Convert(Expression.Convert(leftParam, leftType.ClrType), rightType.ClrType), typeof(object));
                        expression = Expression.Lambda<Func<object?, object?, object?>>(body, leftParam, rightParam);
                    }
                    break;

                default:
                    {
                        var leftArg = Expression.Convert(leftParam, leftType.ClrType);
                        var rightArg = Expression.Convert(rightParam, rightType.ClrType);

                        // C# does not support all operations for SByte and Byte. We need to cast to int and then back to result.
                        var byteResult = resultType == PredefinedTypes.I8 || resultType == PredefinedTypes.U8 ? resultType : null;
                        if (byteResult is not null)
                            resultType = PredefinedTypes.I32;

                        // Ensure numbers are widened before operation.
                        if (resultType.IsNumber)
                        {
                            if (leftType != resultType)
                                leftArg = Expression.Convert(leftArg, resultType.ClrType);

                            if (rightType != resultType)
                                rightArg = Expression.Convert(rightArg, resultType.ClrType);
                        }

                        Expression expr = kind.GetExpressionFactory().Invoke(leftArg, rightArg);
                        if (byteResult is not null)
                            expr = Expression.Convert(expr, byteResult.ClrType);

                        var body = Expression.Convert(expr, typeof(object));
                        expression = Expression.Lambda<Func<object?, object?, object?>>(body, leftParam, rightParam);
                    }
                    break;
            }

            return expression.Compile();
        }
    }

    public static Func<object?, object?, object?> GetOperation(this BoundBinaryExpression expression) => GetOperation(expression.Operator, expression.Left, expression.Right);
}
