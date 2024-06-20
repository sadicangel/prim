using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static FunctionValue EvaluateFunctionBody(BoundFunctionBodyExpression node, InterpreterContext context)
    {
        var functionValue = new FunctionValue(node.FunctionSymbol.Type, FuncFactory.Create(node.FunctionSymbol, node, context));
        context.EvaluatedScope.Replace(node.FunctionSymbol, functionValue);
        return functionValue;
    }
}
