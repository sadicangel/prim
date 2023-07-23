using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Statements;
using CodeAnalysis.Symbols;

namespace CodeAnalysis;

internal sealed class Evaluator : IBoundExpressionVisitor<object?>
{
    private readonly BoundProgram _program;
    private readonly Dictionary<Symbol, object?> _globals;
    private readonly Stack<Dictionary<Symbol, object?>> _locals;

    private object? _lastValue;

    public Evaluator(BoundProgram program, Dictionary<Symbol, object?> globals)
    {
        _program = program;
        _globals = globals;
        _locals = new();
    }

    public object? Evaluate() => _program.Statement is not null ? EvaluateStatement(_program.Statement) : null;

    private object? EvaluateStatement(BoundBlockStatement blockStatement)
    {
        var labelIndices = new Dictionary<LabelSymbol, int>();
        var statements = blockStatement.Statements;
        for (var i = 0; i < statements.Count; ++i)
            if (statements[i] is BoundLabelDeclaration labelStatement)
                labelIndices[labelStatement.Label] = i + 1;

        var index = 0;
        while (index < statements.Count)
        {
            var statement = statements[index];
            switch (statement.NodeKind)
            {
                case BoundNodeKind.NopStatement:
                    index++;
                    break;
                case BoundNodeKind.LabelDeclaration:
                    index++;
                    break;
                case BoundNodeKind.GotoStatement:
                    var gotoStatement = (BoundGotoStatement)statement;
                    index = labelIndices[gotoStatement.Label];
                    break;
                case BoundNodeKind.ConditionalGotoStatement:
                    var conditionalGotoStatement = (BoundConditionalGotoStatement)statement;
                    var condition = (bool)EvaluateExpression(conditionalGotoStatement.Condition)!;
                    index = condition == conditionalGotoStatement.JumpIfTrue ? labelIndices[conditionalGotoStatement.Label] : index + 1;
                    break;
                case BoundNodeKind.ReturnStatement:
                    var returnStatement = (BoundReturnStatement)statement;
                    _lastValue = returnStatement.Expression is null ? null : EvaluateExpression(returnStatement.Expression);
                    return _lastValue;
                default:
                    EvaluateStatement(statement);
                    index++;
                    break;
            }
        }

        return _lastValue;
    }

    private object? EvaluateStatement(BoundStatement statement)
    {
        switch (statement.NodeKind)
        {
            case BoundNodeKind.BlockStatement:
                return EvaluateStatement((BoundBlockStatement)statement);
            case BoundNodeKind.ExpressionStatement:
                var expressionStatement = (BoundExpressionStatement)statement;
                EvaluateExpression(expressionStatement.Expression);
                break;
            case BoundNodeKind.FunctionDeclaration:
                var functionDeclaration = (BoundFunctionDeclaration)statement;
                _globals[functionDeclaration.Function] = functionDeclaration.Body;
                break;
            case BoundNodeKind.VariableDeclaration:
                var variableDeclaration = (BoundVariableDeclaration)statement;
                _globals[variableDeclaration.Variable] = EvaluateExpression(variableDeclaration.Expression);
                break;
            default:
                throw new InvalidOperationException($"Unexpected node of type '{statement.GetType().Name}'");
        }

        return _lastValue;
    }

    private object? EvaluateExpression(BoundExpression expression) => _lastValue = expression.Accept(this);

    object? IBoundExpressionVisitor<object?>.Visit(BoundNeverExpression expression) => null;

    object? IBoundExpressionVisitor<object?>.Visit(BoundIfExpression expression)
    {
        var isTrue = (bool)EvaluateExpression(expression.Condition)!;

        return isTrue ? EvaluateExpression(expression.Then) : EvaluateExpression(expression.Else);
    }

    object? IBoundExpressionVisitor<object?>.Visit(BoundLiteralExpression expression) => expression.Value!;

    private object? GetSymbolValue(Symbol symbol)
    {
        if (!_locals.TryPeek(out var locals) || !locals.TryGetValue(symbol, out var value))
            value = _globals[symbol];

        return value;
    }

    object? IBoundExpressionVisitor<object?>.Visit(BoundSymbolExpression expression)
    {
        var symbol = expression.Symbol;

        switch (symbol.SymbolKind)
        {
            case SymbolKind.Type when BuiltinTypes.TryLookup(symbol.Name, out var type):
                return type;
            case SymbolKind.Function when BuiltinFunctions.TryLookup(symbol.Name, out var function):
                return function;
        }

        if (!_locals.TryPeek(out var locals) || !locals.TryGetValue(symbol, out var value))
            value = _globals[symbol];

        if (symbol.SymbolKind is SymbolKind.Function)
            return symbol;

        return value;
    }

    object? IBoundExpressionVisitor<object?>.Visit(BoundCallExpression expression)
    {
        switch (expression.Function.Name)
        {
            case string name when name == BuiltinFunctions.Scan.Name:
                return Console.ReadLine();

            case string name when name == BuiltinFunctions.Print.Name:
                Console.WriteLine(EvaluateExpression(expression.Arguments[0]));
                return null;

            case string name when name == BuiltinFunctions.ToStr.Name:
                return EvaluateExpression(expression.Arguments[0])?.ToString();

            case string name when name == BuiltinFunctions.IsSame.Name:
                return ReferenceEquals(EvaluateExpression(expression.Arguments[0]), EvaluateExpression(expression.Arguments[1]));

            case string name when name == BuiltinFunctions.Random.Name:
                return Random.Shared.Next((int)EvaluateExpression(expression.Arguments[0])!);

            case string name when name == BuiltinFunctions.TypeOf.Name:
                return expression.Arguments[0].Type;

            case string name when name == BuiltinFunctions.CrlType.Name:
                return (EvaluateExpression(expression.Arguments[0])?.GetType() ?? typeof(void)).Name;

            case string name when _globals.Keys.SingleOrDefault(n => n.Name == name) is FunctionSymbol function:
                return EvaluateFunction(expression);

            default:
                throw new InvalidOperationException($"Undefined function {expression.Function.Name}");
        }

        object? EvaluateFunction(BoundCallExpression expression)
        {
            var locals = new Dictionary<Symbol, object?>();
            for (var i = 0; i < expression.Arguments.Count; ++i)
            {
                var parameter = expression.Function.Parameters[i];
                var parameterValue = EvaluateExpression(expression.Arguments[i]);
                locals[parameter] = parameterValue;
            }

            _locals.Push(locals);
            if (GetSymbolValue(expression.Function) is not BoundStatement statement)
                throw new InvalidOperationException($"Function {expression.Function.Name} was not bound correctly");
            EvaluateStatement(statement);
            _locals.Pop();
            return _lastValue;
        }
    }

    object? IBoundExpressionVisitor<object?>.Visit(BoundConvertExpression expression) => expression.Type.Convert(EvaluateExpression(expression.Expression));

    object? IBoundExpressionVisitor<object?>.Visit(BoundAssignmentExpression expression) => _globals[expression.Variable] = EvaluateExpression(expression.Expression);
    object? IBoundExpressionVisitor<object?>.Visit(BoundCompoundAssignmentExpression expression) => throw new InvalidOperationException($"Unexpected node '{expression.NodeKind}'");

    object? IBoundExpressionVisitor<object?>.Visit(BoundUnaryExpression expression)
    {
        var operation = expression.GetOperation();
        var value = EvaluateExpression(expression.Operand);

        return operation.Invoke(value);
    }

    object? IBoundExpressionVisitor<object?>.Visit(BoundBinaryExpression expression)
    {
        var operation = expression.GetOperation();
        var left = EvaluateExpression(expression.Left);
        var right = EvaluateExpression(expression.Right);

        return operation.Invoke(left, right);
    }
}