using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Binding;

internal sealed class Binder : ISyntaxExpressionVisitor<BoundExpression>, ISyntaxStatementVisitor<BoundStatement>
{
    private readonly DiagnosticBag _diagnostics = new();
    private BoundScope _scope;

    private Binder(BoundScope? parentScope)
    {
        _scope = new BoundScope(parentScope);
        var scope = _scope;
        var level = 0;
        do
        {
            level++;
            scope = scope.Parent;
        }
        while (scope != null);
        //Console.WriteLine($"Scope: {level}");
    }
    public IEnumerable<Diagnostic> Diagnostics { get => _diagnostics; }

    public static BoundProgram BindProgram(BoundGlobalScope globalScope)
    {
        var diagnostics = new DiagnosticBag(globalScope.Diagnostics);
        //var parentScope = CreateParentScope(globalScope);

        //var functions = new Dictionary<FunctionSymbol, BoundStatement>();

        //var scope = globalScope;

        //while (scope is not null)
        //{
        //    foreach (var function in scope.Functions)
        //    {
        //        var binder = new Binder(parentScope);
        //        var body = binder.BindStatement(function.Declaration.Body);
        //        functions[function] = body;
        //        diagnostics.AddRange(binder.Diagnostics);
        //    }
        //    scope = scope.Previous;
        //}

        var globalStatement = new BoundBlockStatement(globalScope.Statements);

        return new BoundProgram(globalStatement, diagnostics);
    }

    internal static BoundGlobalScope BindGlobalScope(CompilationUnit compilationUnit, BoundGlobalScope? previousGlobalScope = null)
    {
        var parentScope = CreateParentScope(previousGlobalScope);
        var binder = new Binder(parentScope);

        var statements = new List<BoundStatement>();

        foreach (var globalStatement in compilationUnit.Nodes.OfType<GlobalStatement>())
        {
            statements.Add(binder.BindStatement(globalStatement.Statement));
        }

        var symbols = binder._scope.Symbols ?? Enumerable.Empty<Symbol>();
        var diagnostics = binder.Diagnostics;
        if (previousGlobalScope is not null)
            diagnostics = previousGlobalScope.Diagnostics.Concat(diagnostics);

        return new BoundGlobalScope(statements, symbols, diagnostics, previousGlobalScope);
    }

    private static BoundScope? CreateParentScope(BoundGlobalScope? previousGlobalScope)
    {
        var stack = new Stack<BoundGlobalScope>();

        while (previousGlobalScope is not null)
        {
            stack.Push(previousGlobalScope);
            previousGlobalScope = previousGlobalScope.Previous;
        }

        var parentScope = CreateRootScope();

        while (stack.Count > 0)
        {
            previousGlobalScope = stack.Pop();
            var scope = new BoundScope(parentScope, previousGlobalScope.Symbols.Concat(parentScope.Symbols ?? Enumerable.Empty<Symbol>()));
            parentScope = scope;
        }

        return parentScope;
    }

    private static BoundScope CreateRootScope()
    {
        var root = new BoundScope(null);
        foreach (var function in BuiltinFunctions.All)
            if (!root.TryDeclare(function))
                throw new InvalidOperationException($"Could not declare built in function {function.Name}");
        foreach (var type in BuiltinTypes.All)
            if (!root.TryDeclare(type))
                throw new InvalidOperationException($"Could not declare built in type {type.Name}");
        return root;
    }

    private BoundStatement BindStatement(Statement statement) => statement.Accept(this);

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(BlockStatement statement)
    {
        _scope = new BoundScope(_scope);
        var boundStatements = statement.Statements.Select(BindStatement).ToList();
        _scope = _scope.Parent ?? throw new InvalidOperationException("Scope cannot be null");
        return new BoundBlockStatement(boundStatements);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(FunctionDeclaration statement)
    {
        var parameters = new ParameterSymbol[statement.Parameters.Count];

        var seenParameterNames = new HashSet<string>();
        for (int i = 0; i < parameters.Length; ++i)
        {
            var parameter = statement.Parameters[i];
            var parameterName = parameter.Identifier.Text;
            var parameterType = GetSymbolOrDefault(parameter.Type, BuiltinTypes.Never);
            if (!seenParameterNames.Add(parameterName))
                _diagnostics.ReportRedeclaration(parameter.Identifier, "parameter");
            parameters[i] = new ParameterSymbol(parameterName, parameterType);

        }
        var type = BuiltinTypes.Never;
        if (!statement.Type.IsMissing)
        {
            type = GetSymbolOrDefault(statement.Type, BuiltinTypes.Never);
            if (type != BuiltinTypes.Void)
                _diagnostics.ReportNotSupported(statement.Type.Span, $"functions of type '{type}'");
        }

        var function = new FunctionSymbol(statement.Identifier.Text, type, parameters);
        if (!_scope.TryDeclare(function))
        {
            //if (type != BuiltinTypes.Never)
            _diagnostics.ReportRedeclaration(statement.Identifier, "function");
        }

        var binder = new Binder(_scope);
        foreach (var parameter in parameters)
        {
            if (!binder._scope.TryDeclare(parameter))
                throw new InvalidOperationException($"Invalid parameter declaration {parameter}");
        }
        var functionBody = binder.BindStatement(statement.Body);

        return new BoundFunctionDeclaration(function, functionBody);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(VariableDeclaration statement)
    {
        var name = statement.Identifier.Text;
        var isReadOnly = statement.Modifier.TokenKind == TokenKind.Const;
        BoundExpression expression;
        if (statement.HasTypeDeclaration)
        {
            if (BuiltinTypes.TryLookup(statement.Type.Text, out var type))
            {
                expression = BindExpression(statement.Expression, type, isExplicit: true);
            }
            else
            {
                _diagnostics.ReportUndefinedName(statement.Type);
                expression = new BoundNeverExpression();
            }
        }
        else
        {
            expression = BindExpression(statement.Expression);
        }
        var variable = new VariableSymbol(name, isReadOnly, expression.Type);

        if (!_scope.TryDeclare(variable))
        {
            _diagnostics.ReportRedeclaration(statement.Identifier, "variable");
        }

        return new BoundVariableDeclaration(variable, expression);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(IfStatement statement)
    {
        var condition = BindExpression(statement.Condition, BuiltinTypes.Bool);
        var then = BindStatement(statement.Then);
        var @else = statement.HasElseClause ? BindStatement(statement.Else) : null;
        return new BoundIfStatement(condition, then, @else);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(WhileStatement statement)
    {
        var condition = BindExpression(statement.Condition, BuiltinTypes.Bool);
        var body = BindStatement(statement.Body);
        return new BoundWhileStatement(condition, body);
    }

    private T GetSymbolOrDefault<T>(Token identifier, T defaultValue) where T : Symbol => TryGetSymbol<T>(identifier, out var symbol) ? symbol : defaultValue;

    private bool TryGetSymbol(Token identifier, [NotNullWhen(true)] out Symbol? symbol)
    {
        if (!_scope.TryLookup(identifier.Text, out symbol))
        {
            _diagnostics.ReportUndefinedName(identifier);

            return false;
        }
        return true;
    }

    private bool TryGetSymbol<T>(Token identifier, [NotNullWhen(true)] out T? symbol) where T : notnull, Symbol
    {
        if (!_scope.TryLookup(identifier.Text, out symbol, out var existingSymbol))
        {
            if (existingSymbol is not null)
                _diagnostics.ReportInvalidSymbol(identifier, Symbol.GetKind<T>(), existingSymbol.Kind);
            else
                _diagnostics.ReportUndefinedName(identifier);

            return false;
        }
        return true;
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(ForStatement statement)
    {
        var lowerBound = BindExpression(statement.LowerBound, BuiltinTypes.I32);
        var upperBound = BindExpression(statement.UpperBound, BuiltinTypes.I32);

        _scope = new BoundScope(_scope);

        VariableSymbol variable;
        if (statement.HasVariableDeclaration)
        {
            variable = new VariableSymbol(statement.Identifier.Text, IsReadOnly: true, BuiltinTypes.I32);
            // Check outer scope for name.
            if (_scope.Parent!.TryLookup(variable.Name, out _) || !_scope.TryDeclare(variable))
                _diagnostics.ReportRedeclaration(statement.Identifier, "variable");
        }
        else
        {
            if (!TryGetSymbol(statement.Identifier, out variable!))
            {
                variable = new VariableSymbol(statement.Identifier.Text, IsReadOnly: true, BuiltinTypes.I32);
            }
            else if (variable.Type != BuiltinTypes.I32)
            {
                _diagnostics.ReportInvalidVariableType(statement.Identifier.Span, BuiltinTypes.I32, variable.Type);
            }
            else if (variable.IsReadOnly)
            {
                _diagnostics.ReportReadOnlyAssignment(statement.Identifier.Span, statement.Identifier.Text);
            }
        }

        var body = BindStatement(statement.Body);

        _scope = _scope.Parent!;

        return new BoundForStatement(variable, lowerBound, upperBound, body);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(ExpressionStatement statement)
    {
        var expression = BindExpression(statement.Expression, allowVoid: true);
        return new BoundExpressionStatement(expression);
    }

    private BoundExpression BindExpression(Expression expression, TypeSymbol targetType, bool isExplicit = false) => BindConversion(expression, targetType, isExplicit);

    private BoundExpression BindExpression(Expression expression) => BindExpression(expression, allowVoid: false);

    private BoundExpression BindExpression(Expression expression, bool allowVoid)
    {
        var result = expression.Accept(this);
        if (!allowVoid && result.Type == BuiltinTypes.Void)
        {
            _diagnostics.ReportInvalidExpressionType(expression.Span, result.Type);
            return new BoundNeverExpression();
        }
        return result;
    }


    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(IfExpression expression)
    {
        var condition = BindExpression(expression.Condition, BuiltinTypes.Bool);
        var then = BindExpression(expression.Then);
        var @else = BindExpression(expression.Else);

        if (then.Type != @else.Type)
        {

        }

        return new BoundIfExpression(condition, then, @else, then.Type);
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(GroupExpression expression) => expression.Expression.Accept(this);

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(UnaryExpression expression)
    {
        var boundOperand = BindExpression(expression.Operand);

        if (boundOperand.Type == BuiltinTypes.Never)
            return new BoundNeverExpression();

        var boundOperator = BoundUnaryOperator.Bind(expression.Operator.TokenKind, boundOperand.Type);
        if (boundOperator is null)
        {
            _diagnostics.ReportUndefinedUnaryOperator(expression.Operator, boundOperand.Type);
            return new BoundNeverExpression();
        }
        return new BoundUnaryExpression(boundOperator, boundOperand);
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(BinaryExpression expression)
    {
        var boundLeft = BindExpression(expression.Left);
        var boundRight = BindExpression(expression.Right);

        if (boundLeft.Type == BuiltinTypes.Never || boundRight.Type == BuiltinTypes.Never)
            return new BoundNeverExpression();

        var resultType = default(TypeSymbol);
        if (expression.Operator.TokenKind is TokenKind.As)
        {
            if (boundRight is not BoundSymbolExpression symbolExpression || BuiltinTypes.Type != symbolExpression.Type)
            {
                _diagnostics.ReportInvalidExpressionType(expression.Right.Span, BuiltinTypes.Type, boundRight.Type);
                return new BoundNeverExpression();
            }

            if (!BuiltinTypes.TryLookup(symbolExpression.Symbol.Name, out resultType))
            {
                _diagnostics.ReportUndefinedType(expression.Right.Span, symbolExpression.Symbol.Name);
                return new BoundNeverExpression();
            }

            // Prevent redundant cast.
            if (boundLeft.Type == resultType)
            {
                return boundLeft;
            }
        }
        var boundOperator = BoundBinaryOperator.Bind(expression.Operator.TokenKind, boundLeft.Type, boundRight.Type, resultType);
        if (boundOperator is null)
        {
            if (expression.Operator.TokenKind is TokenKind.As)
                _diagnostics.ReportInvalidConversion(TextSpan.FromBounds(expression.Operator.Span.Start, expression.Right.Span.End), boundLeft.Type, resultType!);
            else
                _diagnostics.ReportUndefinedBinaryOperator(expression.Operator, boundLeft.Type, boundRight.Type);
            return new BoundNeverExpression();
        }
        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(LiteralExpression expression)
    {
        var value = expression.Value ?? 0;
        return new BoundLiteralExpression(value);
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(NameExpression expression)
    {
        if (expression.Identifier.IsMissing)
        {
            return new BoundNeverExpression();
        }

        if (!TryGetSymbol(expression.Identifier, out var symbol))
        {
            return new BoundNeverExpression();
        }

        return new BoundSymbolExpression(symbol);
    }
    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(CallExpression expression)
    {
        var boundArguments = expression.Arguments.Select(BindExpression).ToArray();

        if (!TryGetSymbol(expression.Identifier, out FunctionSymbol? function))
        {
            return new BoundNeverExpression();
        }

        if (boundArguments.Length != function.Parameters.Count)
        {
            _diagnostics.ReportInvalidArgumentCount(expression.Span, function.Name, function.Parameters.Count, boundArguments.Length);
            return new BoundNeverExpression();
        }

        for (int i = 0; i < boundArguments.Length; ++i)
        {
            var argument = boundArguments[i];
            var parameter = function.Parameters[i];

            if (!argument.Type.IsAssignableTo(parameter.Type))
            {
                _diagnostics.ReportInvalidArgumentType(expression.Arguments[i].Span, parameter.Name, parameter.Type, argument.Type);
                return new BoundNeverExpression();
            }
        }

        return new BoundCallExpression(function, boundArguments);
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(AssignmentExpression expression)
    {
        var boundExpression = BindExpression(expression.Expression);

        if (boundExpression.Type == BuiltinTypes.Never)
            return new BoundNeverExpression();

        if (!TryGetSymbol(expression.Identifier, out VariableSymbol? variable))
        {
            return boundExpression;
        }

        if (variable.IsReadOnly)
        {
            _diagnostics.ReportReadOnlyAssignment(expression.Equal.Span, expression.Identifier.Text);
        }

        if (boundExpression.Type != variable.Type)
        {
            _diagnostics.ReportInvalidConversion(expression.Span, variable.Type, boundExpression.Type);
            return boundExpression;
        }

        return new BoundAssignmentExpression(variable, boundExpression);
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(ConvertExpression expression)
    {
        if (!BuiltinTypes.TryLookup(expression.Type.Text, out var targetType))
        {
            _diagnostics.ReportUndefinedName(expression.Type);
            return new BoundNeverExpression();
        }

        return BindConversion(expression.Expression, targetType, isExplicit: true);
    }

    private BoundExpression BindConversion(Expression expression, TypeSymbol targetType, bool isExplicit)
    {
        var boundExpression = BindExpression(expression);

        if (boundExpression.Type == BuiltinTypes.Never || targetType == BuiltinTypes.Never)
            return boundExpression;

        var conversion = Conversion.Classify(boundExpression.Type, targetType);
        if (!conversion.Exists)
        {
            _diagnostics.ReportInvalidConversion(expression.Span, boundExpression.Type, targetType);
        }
        else if (!conversion.IsIdentity)
        {
            if (conversion.IsImplicit || isExplicit)
                return new BoundConvertExpression(boundExpression, targetType);

            _diagnostics.ReportInvalidImplicitConversion(expression.Span, boundExpression.Type, targetType);
        }
        return boundExpression;
    }
}