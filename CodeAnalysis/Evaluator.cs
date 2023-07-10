using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Statements;
using CodeAnalysis.Symbols;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

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

    public object? Evaluate() => EvaluateStatement(_program.Statement);

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

file static class BoundExpressionExtensions
{
    private readonly record struct UnaryExpressionCacheKey(BoundUnaryOperatorKind Kind, TypeSymbol OperandType);
    private readonly static ConcurrentDictionary<BoundUnaryOperatorKind, ConcurrentDictionary<UnaryExpressionCacheKey, Func<object?, object?>>> UnaryExpressionCache = new();
    private readonly static ConcurrentDictionary<BoundUnaryOperatorKind, Func<Expression, UnaryExpression>> UnaryOperatorCache = new();

    private readonly record struct BinaryExpressionCacheKey(BoundBinaryOperatorKind Kind, TypeSymbol LeftType, TypeSymbol RightType);
    private readonly static ConcurrentDictionary<BoundBinaryOperatorKind, ConcurrentDictionary<BinaryExpressionCacheKey, Func<object?, object?, object?>>> BinaryExpressionCache = new();
    private readonly static ConcurrentDictionary<BoundBinaryOperatorKind, Func<Expression, Expression, BinaryExpression>> BinaryOperatorCache = new();

    public static Func<Expression, UnaryExpression> GetExpressionFactory(this BoundUnaryOperatorKind kind)
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

    public static Func<Expression, Expression, BinaryExpression> GetExpressionFactory(this BoundBinaryOperatorKind kind)
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

    public static Func<object?, object?> GetOperation(this BoundUnaryExpression expression)
    {
        var cacheNode = UnaryExpressionCache.GetOrAdd(expression.Operator.Kind, kind => new ConcurrentDictionary<UnaryExpressionCacheKey, Func<object?, object?>>());

        return cacheNode.GetOrAdd(new UnaryExpressionCacheKey(expression.Operator.Kind, expression.Operand.Type), CreateOperation);

        static Func<object?, object?> CreateOperation(UnaryExpressionCacheKey key)
        {
            var (kind, operandType) = key;
            var operandClrType = operandType.GetClrType();

            var operand = Expression.Parameter(typeof(object), "operand");
            var expr = kind.GetExpressionFactory();
            var body = Expression.Convert(expr.Invoke(Expression.Convert(operand, operandClrType)), typeof(object));
            var expression = Expression.Lambda<Func<object?, object?>>(body, operand);

            return expression.Compile();
        }
    }

    public static Func<object?, object?, object?> GetOperation(this BoundBinaryExpression expression)
    {
        var cacheNode = BinaryExpressionCache.GetOrAdd(expression.Operator.Kind, kind => new ConcurrentDictionary<BinaryExpressionCacheKey, Func<object?, object?, object?>>());

        var rightType = expression.Right.Type;
        if (expression.Operator.Kind is BoundBinaryOperatorKind.ImplicitCast or BoundBinaryOperatorKind.ExplicitCast)
        {
            var symbolExpression = (BoundSymbolExpression)expression.Right;
            if (!BuiltinTypes.TryLookup(symbolExpression.Symbol.Name, out rightType))
                throw new InvalidOperationException($"Could not find type {symbolExpression.Symbol.Name}");
        }

        return cacheNode.GetOrAdd(new BinaryExpressionCacheKey(expression.Operator.Kind, expression.Left.Type, rightType), CreateOperation);

        static Func<object?, object?, object?> CreateOperation(BinaryExpressionCacheKey key)
        {
            var (kind, leftType, rightType) = key;
            var leftClrType = leftType.GetClrType();
            var rightClrType = rightType.GetClrType();
            Expression<Func<object?, object?, object?>> expression;
            if (kind is BoundBinaryOperatorKind.Add && (leftClrType == typeof(string) || rightClrType == typeof(string)))
            {
                var left = Expression.Parameter(typeof(object), "left");
                var right = Expression.Parameter(typeof(object), "right");

                var method = typeof(string).GetMethod(nameof(String.Concat), new[] { typeof(object), typeof(object) });
                if (method is null)
                    throw new InvalidOperationException($"Could not find {nameof(String.Concat)} method");

                var body = Expression.Convert(Expression.Call(instance: null, method, left, right), typeof(object));
                expression = Expression.Lambda<Func<object?, object?, object?>>(body, left, right);
            }
            else if (kind is BoundBinaryOperatorKind.ExplicitCast or BoundBinaryOperatorKind.ImplicitCast)
            {
                var left = Expression.Parameter(typeof(object), "left");
                var right = Expression.Parameter(typeof(object), "right");

                var body = Expression.Convert(Expression.Convert(Expression.Convert(left, leftClrType), rightClrType), typeof(object));
                expression = Expression.Lambda<Func<object?, object?, object?>>(body, left, right);
            }
            else
            {
                var left = Expression.Parameter(typeof(object), "left");
                var right = Expression.Parameter(typeof(object), "right");
                var expr = kind.GetExpressionFactory();
                var body = Expression.Convert(expr.Invoke(Expression.Convert(left, leftClrType), Expression.Convert(right, rightClrType)), typeof(object));
                expression = Expression.Lambda<Func<object?, object?, object?>>(body, left, right);
            }
            return expression.Compile();
        }
    }
}
