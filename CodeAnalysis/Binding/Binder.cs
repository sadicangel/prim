using CodeAnalysis.Lowering;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Binding;

internal sealed class Binder : ISyntaxStatementVisitor<BoundStatement>, ISyntaxExpressionVisitor<BoundExpression>
{
    private readonly DiagnosticBag _diagnostics = new();
    private BoundScope _scope;
    private readonly FunctionSymbol? _function;
    private readonly Stack<(LabelSymbol Break, LabelSymbol Continue)> _loopStack = new();
    private int _labelCount = 0;

    private Binder(BoundScope? parentScope, FunctionSymbol? function = null)
    {
        _scope = new BoundScope(parentScope);
        _function = function;
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

    internal static BoundGlobalScope BindGlobalScope(IReadOnlyList<SyntaxTree> syntaxTrees, BoundGlobalScope? previousGlobalScope = null)
    {
        var parentScope = CreateParentScope(previousGlobalScope);
        var binder = new Binder(parentScope);

        var statements = new List<BoundStatement>();

        foreach (var globalStatement in syntaxTrees.SelectMany(tree => tree.Root.Nodes.OfType<GlobalStatement>()))
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
                _diagnostics.ReportInvalidSymbol(identifier, Symbol.GetKind<T>(), existingSymbol.SymbolKind);
            else
                _diagnostics.ReportUndefinedName(identifier);

            return false;
        }
        return true;
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
            type = GetSymbolOrDefault(statement.Type, BuiltinTypes.Never);

        var function = new FunctionSymbol(statement.Identifier.Text, type, parameters);
        if (!_scope.TryDeclare(function))
        {
            //if (type != BuiltinTypes.Never)
            _diagnostics.ReportRedeclaration(statement.Identifier, "function");
        }

        var binder = new Binder(_scope, function);
        foreach (var parameter in parameters)
        {
            if (!binder._scope.TryDeclare(parameter))
                throw new InvalidOperationException($"Invalid parameter declaration {parameter}");
        }
        var functionBody = binder.BindStatement(statement.Body);

        var loweredBody = Lowerer.Lower(functionBody);

        if (function.Type != BuiltinTypes.Void && function.Type != BuiltinTypes.Never && !ControlFlowGraph.AllPathsReturn(loweredBody))
            binder._diagnostics.ReportNotAllPathsReturn(statement.Identifier.GetLocation());

        _diagnostics.AddRange(binder.Diagnostics);

        return new BoundFunctionDeclaration(function, loweredBody);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(VariableDeclaration statement)
    {
        var name = statement.Identifier.Text;
        var isReadOnly = statement.Modifier.TokenKind == TokenKind.Let;
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
            return new BoundExpressionStatement(new BoundNeverExpression());
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

    private BoundStatement BindLoopBody(Statement body, out LabelSymbol @break, out LabelSymbol @continue)
    {
        _labelCount++;
        @break = new LabelSymbol($"break{_labelCount}");
        @continue = new LabelSymbol($"continue{_labelCount}");
        _loopStack.Push((@break, @continue));
        var boundBody = BindStatement(body);
        _loopStack.Pop();
        return boundBody;
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(WhileStatement statement)
    {
        var condition = BindExpression(statement.Condition, BuiltinTypes.Bool);
        var body = BindLoopBody(statement.Body, out var @break, out var @continue);
        return new BoundWhileStatement(condition, body, @break, @continue);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(ForStatement statement)
    {
        var lowerBound = BindExpression(statement.LowerBound, BuiltinTypes.I32);
        var upperBound = BindExpression(statement.UpperBound, BuiltinTypes.I32);

        var variable = new VariableSymbol(statement.Identifier.Text, IsReadOnly: true, BuiltinTypes.I32);

        // Check outer scope for name. Should this be a warning instead?
        if (_scope.TryLookup(variable.Name, out _))
            _diagnostics.ReportRedeclaration(statement.Identifier, "variable");

        _scope = new BoundScope(_scope);

        if (!_scope.TryDeclare(variable))
            throw new InvalidOperationException($"Could not declare variable '{variable}'");

        var body = BindLoopBody(statement.Body, out var @break, out var @continue);

        _scope = _scope.Parent!;

        return new BoundForStatement(variable, lowerBound, upperBound, body, @break, @continue);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(BreakStatement statement)
    {
        if (!_loopStack.TryPeek(out var jumps))
        {
            _diagnostics.ReportInvalidBreakOrContinue(statement.Break.GetLocation());
            return new BoundExpressionStatement(new BoundNeverExpression());
        }
        return new BoundGotoStatement(jumps.Break);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(ContinueStatement statement)
    {
        if (!_loopStack.TryPeek(out var jumps))
        {
            _diagnostics.ReportInvalidBreakOrContinue(statement.Continue.GetLocation());
            return new BoundExpressionStatement(new BoundNeverExpression());
        }
        return new BoundGotoStatement(jumps.Continue);
    }

    public BoundStatement Visit(ReturnStatement statement)
    {
        if (_function is null)
        {
            _diagnostics.ReportInvalidReturn(statement.Return.GetLocation());
            return new BoundExpressionStatement(new BoundNeverExpression());
        }
        else if (_function.Type == BuiltinTypes.Void)
        {
            if (statement.Expression is not null)
            {
                _diagnostics.ReportInvalidReturnExpression(statement.Expression.GetLocation(), _function.Name);
                return new BoundReturnStatement(new BoundNeverExpression());
            }
            return new BoundReturnStatement();
        }
        else if (statement.Expression is null)
        {
            _diagnostics.ReportInvalidReturnExpression(statement.Return.GetLocation(), _function.Name, _function.Type);
            return new BoundReturnStatement(new BoundNeverExpression());
        }
        else
        {
            var expression = BindConversion(statement.Expression, _function.Type, isExplicit: false);
            return new BoundReturnStatement(expression);
        }
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
            _diagnostics.ReportInvalidExpressionType(expression.GetLocation(), result.Type);
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
                _diagnostics.ReportInvalidExpressionType(expression.Right.GetLocation(), BuiltinTypes.Type, boundRight.Type);
                return new BoundNeverExpression();
            }

            if (!BuiltinTypes.TryLookup(symbolExpression.Symbol.Name, out resultType))
            {
                _diagnostics.ReportUndefinedType(expression.Right.GetLocation(), symbolExpression.Symbol.Name);
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
                _diagnostics.ReportInvalidConversion(new TextLocation(expression.SyntaxTree.Text, TextSpan.FromBounds(expression.Operator.Span.Start, expression.Right.Span.End)), boundLeft.Type, resultType!);
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

        if (!TryGetSymbol(expression.Identifier, out FunctionSymbol? function))
        {
            return new BoundNeverExpression();
        }

        if (expression.Arguments.Count != function.Parameters.Count)
        {
            TextLocation location;
            if (expression.Arguments.Count > function.Parameters.Count)
            {
                SyntaxNode firstExceedingNode = function.Parameters.Count > 0
                    ? expression.Arguments.GetSeparator(function.Parameters.Count - 1)
                    : expression.Arguments[0];
                var lastExceedingArgument = expression.Arguments[^1];
                location = new TextLocation(expression.SyntaxTree.Text, TextSpan.FromBounds(firstExceedingNode.Span.Start, lastExceedingArgument.Span.End));
            }
            else
            {
                location = expression.CloseParenthesis.GetLocation();
            }
            _diagnostics.ReportInvalidArgumentCount(location, function.Name, function.Parameters.Count, expression.Arguments.Count);
            return new BoundNeverExpression();
        }

        var boundArguments = expression.Arguments.Select(BindExpression).ToArray();

        for (int i = 0; i < boundArguments.Length; ++i)
        {
            var argument = boundArguments[i];
            var parameter = function.Parameters[i];

            if (!argument.Type.IsAssignableTo(parameter.Type))
            {
                _diagnostics.ReportInvalidArgumentType(expression.Arguments[i].GetLocation(), parameter.Name, parameter.Type, argument.Type);
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
            _diagnostics.ReportReadOnlyAssignment(expression.Equal.GetLocation(), expression.Identifier.Text);
        }

        if (boundExpression.Type != variable.Type)
        {
            _diagnostics.ReportInvalidConversion(expression.GetLocation(), variable.Type, boundExpression.Type);
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
            _diagnostics.ReportInvalidConversion(expression.GetLocation(), boundExpression.Type, targetType);
        }
        else if (!conversion.IsIdentity)
        {
            if (conversion.IsImplicit || isExplicit)
                return new BoundConvertExpression(boundExpression, targetType);

            _diagnostics.ReportInvalidImplicitConversion(expression.GetLocation(), boundExpression.Type, targetType);
        }
        return boundExpression;
    }
}