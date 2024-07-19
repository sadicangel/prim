using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static LambdaValue EvaluateFunctionAssignmentExpression(BoundFunctionAssignmentExpression node, InterpreterContext context)
    {
        var lambda = new LambdaValue(
            node.FunctionSymbol.LambdaType,
            FuncFactory.Create(node.FunctionSymbol.LambdaType, node, context));
        context.EvaluatedScope.Replace(node.FunctionSymbol, lambda);
        return lambda;
    }
}
