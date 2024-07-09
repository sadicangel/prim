using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static FunctionValue EvaluateFunctionDeclaration(BoundFunctionDeclaration node, InterpreterContext context)
    {
        // TODO: Can we merge Function and Variable values?
        var functionValue = new FunctionValue(node.FunctionSymbol.FunctionType, FuncFactory.Create(node.FunctionSymbol, node.Body, context));
        context.EvaluatedScope.Declare(node.FunctionSymbol, functionValue);
        return functionValue;
    }
}
