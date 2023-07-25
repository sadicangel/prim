using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Statements;
using CodeAnalysis.Lowering;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Statements;
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
    }
    public IReadOnlyDiagnosticBag Diagnostics { get => _diagnostics; }

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

        var globalStatement = globalScope.Statements.Count > 0
            ? new BoundBlockStatement(globalScope.Statements[0].Syntax, globalScope.Statements)
            : null;

        var program = new BoundProgram(globalStatement, diagnostics);

        program = Lowerer.Lower(program);

        return program;
    }

    internal static BoundGlobalScope BindGlobalScope(IReadOnlyList<SyntaxTree> syntaxTrees, BoundGlobalScope? previousGlobalScope = null)
    {
        var parentScope = CreateParentScope(previousGlobalScope);
        var binder = new Binder(parentScope);

        var statements = new List<BoundStatement>();

        // We have 2 types of global nodes: Declarations and Statements.
        var groupedNodes = syntaxTrees
            .SelectMany(tree => tree.Root.Nodes)
            .GroupBy(node => node.NodeKind)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        // We want to make sure declaration run before statements.
        if (groupedNodes.TryGetValue(SyntaxNodeKind.GlobalDeclaration, out var globalDeclarations))
            foreach (var global in globalDeclarations.Cast<GlobalDeclaration>())
                statements.Add(binder.BindStatement(global.Declaration));

        if (groupedNodes.TryGetValue(SyntaxNodeKind.GlobalStatement, out var globalStatements))
            foreach (var global in globalStatements.Cast<GlobalStatement>())
                statements.Add(binder.BindStatement(global.Statement));

        var symbols = binder._scope.Symbols ?? Enumerable.Empty<Symbol>();
        var diagnostics = binder.Diagnostics;
        //if (previousGlobalScope is not null)
        //    diagnostics = previousGlobalScope.Diagnostics.Concat(diagnostics);

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
        foreach (var type in PredefinedTypes.All)
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
        return new BoundBlockStatement(statement, boundStatements);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(FunctionDeclaration statement)
    {
        var parameters = new ParameterSymbol[statement.Parameters.Count];

        var seenParameterNames = new HashSet<string>();
        for (int i = 0; i < parameters.Length; ++i)
        {
            var parameter = statement.Parameters[i];
            var parameterName = parameter.Identifier.Text;
            var parameterType = GetSymbolOrDefault(parameter.Type, PredefinedTypes.Never);
            if (!seenParameterNames.Add(parameterName))
                _diagnostics.ReportRedeclaration(parameter.Identifier, "parameter");
            parameters[i] = new ParameterSymbol(parameterName, parameterType);

        }
        var type = PredefinedTypes.Never;
        if (!statement.Type.IsMissing)
            type = GetSymbolOrDefault(statement.Type, PredefinedTypes.Never);

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

        if (function.Type != PredefinedTypes.Void && function.Type != PredefinedTypes.Never && !ControlFlowGraph.AllPathsReturn(loweredBody))
            binder._diagnostics.ReportNotAllPathsReturn(statement.Identifier.GetLocation());

        _diagnostics.AddRange(binder.Diagnostics);

        return new BoundFunctionDeclaration(statement, function, loweredBody);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(VariableDeclaration statement)
    {
        var name = statement.Identifier.Text;
        var isReadOnly = statement.Modifier.TokenKind == TokenKind.Let;
        BoundExpression expression;
        if (statement.HasTypeDeclaration)
        {
            if (PredefinedTypes.TryLookup(statement.Type.Text, out var type))
            {
                expression = BindExpression(statement.Expression, type, isExplicit: true);
            }
            else
            {
                _diagnostics.ReportUndefinedName(statement.Type);
                expression = new BoundNeverExpression(statement);
            }
        }
        else
        {
            expression = BindExpression(statement.Expression);
        }

        var variable = new VariableSymbol(name, isReadOnly, expression.Type, isReadOnly ? expression.ConstantValue : null);

        if (!_scope.TryDeclare(variable))
        {
            _diagnostics.ReportRedeclaration(statement.Identifier, "variable");
            return new BoundExpressionStatement(statement, new BoundNeverExpression(statement));
        }

        return new BoundVariableDeclaration(statement, variable, expression);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(IfStatement statement)
    {
        var condition = BindExpression(statement.Condition, PredefinedTypes.Bool);
        if (condition.ConstantValue is not null)
        {
            if (!(bool)condition.ConstantValue.Value!)
                _diagnostics.ReportUnreachableCode(statement.Then);
            else if (statement.HasElseClause)
                _diagnostics.ReportUnreachableCode(statement.Else);
        }
        var then = BindStatement(statement.Then);
        var @else = statement.HasElseClause ? BindStatement(statement.Else) : null;
        return new BoundIfStatement(statement, condition, then, @else);
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
        var condition = BindExpression(statement.Condition, PredefinedTypes.Bool);
        if (condition.ConstantValue is not null)
        {
            if (!(bool)condition.ConstantValue.Value!)
                _diagnostics.ReportUnreachableCode(statement.Body);
        }
        var body = BindLoopBody(statement.Body, out var @break, out var @continue);
        return new BoundWhileStatement(statement, condition, body, @break, @continue);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(ForStatement statement)
    {
        var lowerBound = BindExpression(statement.LowerBound, PredefinedTypes.I32);
        var upperBound = BindExpression(statement.UpperBound, PredefinedTypes.I32);

        if (lowerBound.ConstantValue is not null && upperBound.ConstantValue is not null)
        {
            var loBound = (int)lowerBound.ConstantValue.Value!;
            var hiBound = (int)upperBound.ConstantValue.Value!;
            if (loBound >= hiBound)
                _diagnostics.ReportUnreachableCode(statement.Body);
        }

        var variable = new VariableSymbol(statement.Identifier.Text, IsReadOnly: true, PredefinedTypes.I32);

        // Check outer scope for name. Should this be a warning instead?
        if (_scope.TryLookup(variable.Name, out _))
            _diagnostics.ReportRedeclaration(statement.Identifier, "variable");

        _scope = new BoundScope(_scope);

        if (!_scope.TryDeclare(variable))
            throw new InvalidOperationException($"Could not declare variable '{variable}'");

        var body = BindLoopBody(statement.Body, out var @break, out var @continue);

        _scope = _scope.Parent!;

        return new BoundForStatement(statement, variable, lowerBound, upperBound, body, @break, @continue);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(BreakStatement statement)
    {
        if (!_loopStack.TryPeek(out var jumps))
        {
            _diagnostics.ReportInvalidBreakOrContinue(statement.Break.GetLocation());
            return new BoundExpressionStatement(statement, new BoundNeverExpression(statement));
        }
        return new BoundGotoStatement(statement, jumps.Break);
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(ContinueStatement statement)
    {
        if (!_loopStack.TryPeek(out var jumps))
        {
            _diagnostics.ReportInvalidBreakOrContinue(statement.Continue.GetLocation());
            return new BoundExpressionStatement(statement, new BoundNeverExpression(statement));
        }
        return new BoundGotoStatement(statement, jumps.Continue);
    }

    public BoundStatement Visit(ReturnStatement statement)
    {
        if (_function is null)
        {
            _diagnostics.ReportInvalidReturn(statement.Return.GetLocation());
            return new BoundExpressionStatement(statement, new BoundNeverExpression(statement));
        }
        else if (_function.Type == PredefinedTypes.Void)
        {
            if (statement.Expression is not null)
            {
                _diagnostics.ReportInvalidReturnExpression(statement.Expression.GetLocation(), _function.Name);
                return new BoundReturnStatement(statement, new BoundNeverExpression(statement));
            }
            return new BoundReturnStatement(statement);
        }
        else if (statement.Expression is null)
        {
            _diagnostics.ReportInvalidReturnExpression(statement.Return.GetLocation(), _function.Name, _function.Type);
            return new BoundReturnStatement(statement, new BoundNeverExpression(statement));
        }
        else
        {
            var expression = BindConversion(statement.Expression, _function.Type, allowExplicit: false);
            return new BoundReturnStatement(statement, expression);
        }
    }

    BoundStatement ISyntaxStatementVisitor<BoundStatement>.Visit(ExpressionStatement statement)
    {
        var expression = BindExpression(statement.Expression, allowVoid: true);
        return new BoundExpressionStatement(statement, expression);
    }

    private BoundExpression BindExpression(Expression expression, TypeSymbol targetType, bool isExplicit = false) => BindConversion(expression, targetType, isExplicit);

    private BoundExpression BindExpression(Expression expression) => BindExpression(expression, allowVoid: false);

    private BoundExpression BindExpression(Expression expression, bool allowVoid)
    {
        var result = expression.Accept(this);
        if (!allowVoid && result.Type == PredefinedTypes.Void)
        {
            _diagnostics.ReportInvalidExpressionType(expression.GetLocation(), result.Type);
            return new BoundNeverExpression(expression);
        }
        return result;
    }


    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(IfExpression expression)
    {
        throw new NotSupportedException(nameof(IfExpression));
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(GroupExpression expression) => expression.Expression.Accept(this);

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(UnaryExpression expression)
    {
        var boundOperand = BindExpression(expression.Operand);

        if (boundOperand.Type == PredefinedTypes.Never)
            return new BoundNeverExpression(expression);

        var boundOperator = BoundUnaryOperator.Bind(expression.Operator.TokenKind, boundOperand.Type);
        if (boundOperator is null)
        {
            _diagnostics.ReportUndefinedUnaryOperator(expression.Operator, boundOperand.Type);
            return new BoundNeverExpression(expression);
        }
        return new BoundUnaryExpression(expression, boundOperator, boundOperand);
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(BinaryExpression expression)
    {
        var boundLeft = BindExpression(expression.Left);
        var boundRight = BindExpression(expression.Right);

        if (boundLeft.Type == PredefinedTypes.Never || boundRight.Type == PredefinedTypes.Never)
            return new BoundNeverExpression(expression);

        var resultType = default(TypeSymbol);
        if (expression.Operator.TokenKind is TokenKind.As)
        {
            if (boundRight is not BoundSymbolExpression symbolExpression || PredefinedTypes.Type != symbolExpression.Type)
            {
                _diagnostics.ReportInvalidExpressionType(expression.Right.GetLocation(), PredefinedTypes.Type, boundRight.Type);
                return new BoundNeverExpression(expression);
            }

            if (!PredefinedTypes.TryLookup(symbolExpression.Symbol.Name, out resultType))
            {
                _diagnostics.ReportUndefinedType(expression.Right.GetLocation(), symbolExpression.Symbol.Name);
                return new BoundNeverExpression(expression);
            }

            // Warn and prevent redundant cast.
            if (boundLeft.Type == resultType)
            {
                _diagnostics.ReportRedundantConversion(new TextLocation(expression.SyntaxTree.Text, TextSpan.FromBounds(expression.Operator.Span, expression.Right.Span)));
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
            return new BoundNeverExpression(expression);
        }
        return new BoundBinaryExpression(expression, boundLeft, boundOperator, boundRight);
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(LiteralExpression expression)
    {
        var value = expression.Value ?? 0;
        return new BoundLiteralExpression(expression, value);
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(NameExpression expression)
    {
        if (expression.Identifier.IsMissing)
        {
            return new BoundNeverExpression(expression);
        }

        if (!TryGetSymbol(expression.Identifier, out var symbol))
        {
            return new BoundNeverExpression(expression);
        }

        return new BoundSymbolExpression(expression, symbol);
    }
    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(CallExpression expression)
    {

        if (!TryGetSymbol(expression.Identifier, out FunctionSymbol? function))
        {
            return new BoundNeverExpression(expression);
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
            return new BoundNeverExpression(expression);
        }

        var boundArguments = expression.Arguments.Select(BindExpression).ToArray();

        for (int i = 0; i < boundArguments.Length; ++i)
        {
            var argument = boundArguments[i];
            var parameter = function.Parameters[i];

            if (!argument.Type.IsAssignableTo(parameter.Type))
            {
                _diagnostics.ReportInvalidArgumentType(expression.Arguments[i].GetLocation(), parameter.Name, parameter.Type, argument.Type);
                return new BoundNeverExpression(expression);
            }
        }

        return new BoundCallExpression(expression, function, boundArguments);
    }

    BoundExpression ISyntaxExpressionVisitor<BoundExpression>.Visit(AssignmentExpression expression)
    {
        var boundExpression = BindExpression(expression.Expression);

        if (boundExpression.Type == PredefinedTypes.Never)
            return new BoundNeverExpression(expression);

        if (!TryGetSymbol(expression.Identifier, out VariableSymbol? variable))
        {
            return boundExpression;
        }

        if (variable.IsReadOnly)
        {
            _diagnostics.ReportReadOnlyAssignment(expression.Assign.GetLocation(), expression.Identifier.Text);
        }

        // Compound assignment?
        if (expression.Assign.TokenKind is not TokenKind.Equal)
        {
            var operatorKind = expression.Assign.TokenKind.GetBinaryOperatorOfAssignmentOperator();
            var boundOperator = BoundBinaryOperator.Bind(operatorKind, variable.Type, boundExpression.Type, variable.Type);
            if (boundOperator is null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(expression.Assign, variable.Type, boundExpression.Type);
                return new BoundNeverExpression(expression);
            }

            return new BoundCompoundAssignmentExpression(expression, variable, boundOperator, boundExpression);
        }

        if (boundExpression.Type != variable.Type)
        {
            _diagnostics.ReportInvalidConversion(expression.GetLocation(), variable.Type, boundExpression.Type);
            return boundExpression;
        }

        return new BoundAssignmentExpression(expression, variable, boundExpression);

    }

    private BoundExpression BindConversion(Expression expression, TypeSymbol targetType, bool allowExplicit)
    {
        var boundExpression = BindExpression(expression);

        if (boundExpression.Type == PredefinedTypes.Never || targetType == PredefinedTypes.Never)
            return boundExpression;

        var conversion = Conversion.Classify(boundExpression.Type, targetType);
        if (!conversion.Exists)
        {
            _diagnostics.ReportInvalidConversion(expression.GetLocation(), boundExpression.Type, targetType);
            return new BoundNeverExpression(expression);
        }
        else if (!conversion.IsIdentity)
        {
            if (conversion.IsImplicit || allowExplicit)
                return new BoundConvertExpression(expression, boundExpression, targetType);

            _diagnostics.ReportInvalidImplicitConversion(expression.GetLocation(), boundExpression.Type, targetType);
        }
        return boundExpression;
    }
}