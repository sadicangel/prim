using System.Linq.Expressions;
using System.Reflection;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static class FuncFactory
    {
        private static readonly MethodInfo s_pushScopeMethodInfo =
            typeof(InterpreterContext).GetMethod(nameof(InterpreterContext.PushScope))!;
        private static readonly MethodInfo s_disposeScopeMethodInfo =
            typeof(InterpreterContext.TempScope).GetMethod(nameof(InterpreterContext.TempScope.Dispose))!;
        private static readonly PropertyInfo s_evaluatedScopePropertyInfo =
            typeof(InterpreterContext).GetProperty(nameof(InterpreterContext.EvaluatedScope))!;
        private static readonly MethodInfo s_declareSymbolMethodInfo =
            typeof(EvaluatedScope).GetMethod(nameof(EvaluatedScope.Declare))!;
        private static readonly MethodInfo s_evaluateNodeMethodInfo =
            typeof(Interpreter).GetMethod(nameof(EvaluateNode), BindingFlags.NonPublic | BindingFlags.Static)!;

        public static Delegate Create(FunctionSymbol function, BoundFunctionBodyExpression body, InterpreterContext context)
        {
            var disposableVar = Expression.Variable(typeof(InterpreterContext.TempScope), "<$>disposable");
            var contextConst = Expression.Constant(context, typeof(InterpreterContext));
            var evaluatedScope = Expression.Property(contextConst, s_evaluatedScopePropertyInfo);
            var parameters = function.Type.Parameters.Select(p => Expression.Parameter(typeof(PrimValue), p.Name)).ToArray();
            var variables = body.ParameterSymbols.Select(p => Expression.Constant(p, typeof(VariableSymbol))).ToArray();
            var value = Expression.Variable(typeof(PrimValue), "value");
            var expression = Expression.Constant(body.Expression, typeof(BoundExpression));

            return Expression.Lambda(
                Expression.Block(
                    [value, disposableVar],
                    Expression.Assign(
                        disposableVar,
                        Expression.Call(contextConst, s_pushScopeMethodInfo)),
                    Expression.TryFinally(
                        Expression.Block(
                            Expression.Block(function.Type.Parameters.Select((p, i) =>
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
