﻿using CodeAnalysis.Binding.Expressions;
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

    private readonly record struct BinaryExpressionCacheKey(BoundBinaryOperatorKind Kind, TypeSymbol LeftType, TypeSymbol RightType);
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
            var left = Expression.Parameter(typeof(Expression), "left");
            var right = Expression.Parameter(typeof(Expression), "right");
            var method = typeof(Expression).GetMethod(kind.GetLinqExpressionName(), BindingFlags.Public | BindingFlags.Static, new[] { typeof(Expression), typeof(Expression) });
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

        return cacheNode.GetOrAdd(new BinaryExpressionCacheKey(@operator.Kind, left.Type, rightType), CreateOperation);

        static Func<object?, object?, object?> CreateOperation(BinaryExpressionCacheKey key)
        {
            var (kind, leftType, rightType) = key;
            Expression<Func<object?, object?, object?>> expression;
            if (kind is BoundBinaryOperatorKind.Add && (leftType.ClrType == typeof(string) || rightType.ClrType == typeof(string)))
            {
                var left = Expression.Parameter(typeof(object), "left");
                var right = Expression.Parameter(typeof(object), "right");

                var method = typeof(string).GetMethod(nameof(String.Concat), new[] { typeof(object), typeof(object) });
                if (method is null)
                    throw new InvalidOperationException($"Could not find {nameof(String.Concat)} method");

                var body = Expression.Convert(Expression.Call(instance: null, method, left, right), typeof(object));
                expression = Expression.Lambda<Func<object?, object?, object?>>(body, left, right);
            }
            else if (kind is BoundBinaryOperatorKind.Exponent)
            {
                var left = Expression.Parameter(typeof(object), "left");
                var right = Expression.Parameter(typeof(object), "right");

                var method = typeof(Math).GetMethod(nameof(Math.Pow), new[] { typeof(double), typeof(double) });
                if (method is null)
                    throw new InvalidOperationException($"Could not find {nameof(Math.Pow)} method");

                var leftConv = Expression.Convert(Expression.Convert(left, leftType.ClrType), typeof(double));
                var rightConv = Expression.Convert(Expression.Convert(right, rightType.ClrType), typeof(double));

                var body = Expression.Convert(Expression.Convert(Expression.Call(instance: null, method, leftConv, rightConv), leftType.ClrType), typeof(object));
                expression = Expression.Lambda<Func<object?, object?, object?>>(body, left, right);
            }
            else if (kind is BoundBinaryOperatorKind.ExplicitCast or BoundBinaryOperatorKind.ImplicitCast)
            {
                var left = Expression.Parameter(typeof(object), "left");
                var right = Expression.Parameter(typeof(object), "right");

                var body = Expression.Convert(Expression.Convert(Expression.Convert(left, leftType.ClrType), rightType.ClrType), typeof(object));
                expression = Expression.Lambda<Func<object?, object?, object?>>(body, left, right);
            }
            else
            {
                var left = Expression.Parameter(typeof(object), "left");
                var right = Expression.Parameter(typeof(object), "right");
                var expr = kind.GetExpressionFactory();
                var body = Expression.Convert(expr.Invoke(Expression.Convert(left, leftType.ClrType), Expression.Convert(right, rightType.ClrType)), typeof(object));
                expression = Expression.Lambda<Func<object?, object?, object?>>(body, left, right);
            }
            return expression.Compile();
        }
    }

    public static Func<object?, object?, object?> GetOperation(this BoundBinaryExpression expression) => GetOperation(expression.Operator, expression.Left, expression.Right);
}
