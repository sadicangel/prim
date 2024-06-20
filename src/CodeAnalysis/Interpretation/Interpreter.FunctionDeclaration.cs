using System.Linq.Expressions;
using System.Reflection;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static FunctionValue EvaluateFunctionDeclaration(BoundFunctionDeclaration node, InterpreterContext context)
    {
        var functionValue = new FunctionValue(node.NameSymbol.Type, FuncFactory.Create(node.NameSymbol, node.Expression, context));
        context.EvaluatedScope.Declare(node.NameSymbol, functionValue);
        return functionValue;
    }

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
        private static readonly ConstructorInfo s_variableSymbolConstructorInfo =
            typeof(VariableSymbol).GetConstructor([typeof(SyntaxNode), typeof(string), typeof(PrimType), typeof(bool)])!;
        private static readonly MethodInfo s_evaluateNodeMethodInfo =
            typeof(Interpreter).GetMethod(nameof(Interpreter.EvaluateNode), BindingFlags.NonPublic | BindingFlags.Static)!;

        public static Delegate Create(FunctionSymbol function, BoundExpression expression, InterpreterContext context)
        {
            var disposableVar = Expression.Variable(typeof(InterpreterContext.TempScope), "<$>disposable");
            var contextConst = Expression.Constant(context, typeof(InterpreterContext));
            var evaluatedScope = Expression.Property(contextConst, s_evaluatedScopePropertyInfo);
            var parameters = function.Type.Parameters.Select(p => Expression.Parameter(typeof(PrimValue), p.Name)).ToArray();
            var value = Expression.Variable(typeof(PrimValue), "value");
            var node = Expression.Constant(expression, typeof(BoundExpression));

            var body = Expression.Block(
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
                                Expression.New(
                                    s_variableSymbolConstructorInfo,
                                    Expression.Constant(function.Syntax, typeof(SyntaxNode)),
                                    Expression.Constant(p.Name),
                                    Expression.Constant(p.Type),
                                    Expression.Constant(false)),
                                parameters[i]))),
                        Expression.Assign(value, Expression.Call(s_evaluateNodeMethodInfo, node, contextConst))),
                    Expression.Call(disposableVar, s_disposeScopeMethodInfo)),
                value
            );
            var lambda = Expression.Lambda(body, parameters);
            var func = lambda.Compile();
            return func;
        }
    }
}
