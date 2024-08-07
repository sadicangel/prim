﻿using System.Linq.Expressions;
using System.Reflection;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static class FuncFactory
    {
        private static readonly MethodInfo s_pushScopeMethodInfo;
        private static readonly MethodInfo s_disposeScopeMethodInfo;
        private static readonly PropertyInfo s_evaluatedScopePropertyInfo;
        private static readonly MethodInfo s_declareSymbolMethodInfo;
        private static readonly MethodInfo s_evaluateNodeMethodInfo;

        static FuncFactory()
        {
            s_pushScopeMethodInfo = typeof(Context).GetMethod(nameof(Context.PushScope))
                ?? throw new InvalidOperationException($"Reflection failed for {nameof(s_pushScopeMethodInfo)}");
            s_disposeScopeMethodInfo = typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose))
                ?? throw new InvalidOperationException($"Reflection failed for {nameof(s_disposeScopeMethodInfo)}");
            s_evaluatedScopePropertyInfo = typeof(Context).GetProperty(nameof(Context.EvaluatedScope))
                ?? throw new InvalidOperationException($"Reflection failed for {nameof(s_evaluatedScopePropertyInfo)}");
            s_declareSymbolMethodInfo = typeof(ScopeValue).GetMethod(nameof(ScopeValue.Declare))
                ?? throw new InvalidOperationException($"Reflection failed for {nameof(s_declareSymbolMethodInfo)}");
            s_evaluateNodeMethodInfo = typeof(Interpreter).GetMethod(nameof(EvaluateNode), BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException($"Reflection failed for {nameof(s_evaluateNodeMethodInfo)}");
        }

        public static Delegate Create(LambdaTypeSymbol lambdaType, BoundExpression body, Context context)
        {
            var disposableVar = Expression.Variable(typeof(IDisposable), "<$>disposable");
            var contextConst = Expression.Constant(context, typeof(Context));
            var evaluatedScope = Expression.Property(contextConst, s_evaluatedScopePropertyInfo);
            var parameters = lambdaType.Parameters.Select(p => Expression.Parameter(typeof(PrimValue), p.Name)).ToArray();
            var variables = lambdaType.Parameters.Select(p => Expression.Constant(p, typeof(VariableSymbol))).ToArray();
            var value = Expression.Variable(typeof(PrimValue), "value");
            var expression = Expression.Constant(body, typeof(BoundExpression));

            return Expression.Lambda(
                Expression.Block(
                    [value, disposableVar],
                    Expression.Assign(
                        disposableVar,
                        Expression.Call(contextConst, s_pushScopeMethodInfo, Expression.Constant(null, typeof(ModuleValue)))),
                    Expression.TryFinally(
                        Expression.Block(
                            Expression.Block(lambdaType.Parameters.Select((p, i) =>
                                Expression.Call(
                                    evaluatedScope,
                                    s_declareSymbolMethodInfo,
                                    variables[i],
                                    parameters[i]))),
                            Expression.Assign(value, Expression.Call(s_evaluateNodeMethodInfo, expression, contextConst))),
                        Expression.Call(disposableVar, s_disposeScopeMethodInfo)),
                    value),
                parameters)
                .Compile();
        }
    }
}
