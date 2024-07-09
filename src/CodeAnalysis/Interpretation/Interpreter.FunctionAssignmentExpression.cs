using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static FunctionValue EvaluateFunctionAssignmentExpression(BoundFunctionAssignmentExpression node, InterpreterContext context)
    {
        var functionValue = new FunctionValue(node.FunctionSymbol.Type, FuncFactory.Create(node.FunctionSymbol, node, context));
        context.EvaluatedScope.Replace(node.FunctionSymbol, functionValue);
        return functionValue;
    }
}
