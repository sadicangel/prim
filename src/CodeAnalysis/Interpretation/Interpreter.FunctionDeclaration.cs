using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static FunctionValue EvaluateFunctionDeclaration(BoundFunctionDeclaration node, InterpreterContext context)
    {
        var functionValue = new FunctionValue(node.NameSymbol.Type, FuncFactory.Create(node.NameSymbol, node.Body, context));
        context.EvaluatedScope.Declare(node.NameSymbol, functionValue);
        return functionValue;
    }
}
