using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Binding;

internal sealed class Binder : IExpressionVisitor<BoundExpression>, IStatementVisitor<BoundStatement>
{
    private readonly DiagnosticBag _diagnostics = new();
    private BoundScope _scope;

    private Binder(BoundScope? parentScope = null)
    {
        _scope = new BoundScope(parentScope);
    }
    public IEnumerable<Diagnostic> Diagnostics { get => _diagnostics; }

    internal static BoundGlobalScope BindGlobalScope(CompilationUnit compilationUnit, BoundGlobalScope? previousScope = null)
    {
        var parentScope = CreateParentScope(previousScope);
        var binder = new Binder(parentScope);
        var expression = binder.BindStatement(compilationUnit.Statement);
        var variables = binder._scope.Variables;
        var diagnostics = binder.Diagnostics;

        return new BoundGlobalScope(diagnostics, variables, expression, previousScope);
    }

    private static BoundScope? CreateParentScope(BoundGlobalScope? previous)
    {
        var stack = new Stack<BoundGlobalScope>();

        while (previous is not null)
        {
            stack.Push(previous);
            previous = previous.Previous;
        }

        var parent = CreateRootScope();

        while (stack.Count > 0)
        {
            previous = stack.Pop();
            var scope = new BoundScope(parent);
            foreach (var v in previous.Variables)
                scope.TryDeclare(v);
            parent = scope;
        }

        return parent;
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

    BoundStatement IStatementVisitor<BoundStatement>.Accept(BlockStatement statement)
    {
        var boundStatements = new List<BoundStatement>();
        _scope = new BoundScope(_scope);
        foreach (var s in statement.Statements)
            boundStatements.Add(BindStatement(s));
        _scope = _scope.Parent ?? throw new InvalidOperationException("Scope cannot be null");
        return new BoundBlockStatement(boundStatements);
    }
    BoundStatement IStatementVisitor<BoundStatement>.Accept(DeclarationStatement statement)
    {
        var name = statement.IdentifierToken.Text;
        var isReadOnly = statement.StorageToken.Kind == TokenKind.Const;
        BoundExpression expression;
        if (statement.HasTypeDeclaration)
        {
            if (BuiltinTypes.TryLookup(statement.TypeToken.Text, out var type))
            {
                expression = BindExpression(statement.Expression, type, isExplicit: true);
            }
            else
            {
                _diagnostics.ReportUndefinedName(statement.TypeToken);
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
            _diagnostics.ReportRedeclaration(statement.IdentifierToken);
        }

        return new BoundDeclarationStatement(variable, expression);
    }

    BoundStatement IStatementVisitor<BoundStatement>.Accept(IfStatement statement)
    {
        var condition = BindExpression(statement.Condition, BuiltinTypes.Bool);
        var then = BindStatement(statement.Then);
        var @else = statement.HasElseClause ? BindStatement(statement.Else) : null;
        return new BoundIfStatement(condition, then, @else);
    }

    BoundStatement IStatementVisitor<BoundStatement>.Accept(WhileStatement statement)
    {
        var condition = BindExpression(statement.Condition, BuiltinTypes.Bool);
        var body = BindStatement(statement.Body);
        return new BoundWhileStatement(condition, body);
    }

    private bool TryLookupSymbol(Token identifierToken, [NotNullWhen(true)] out Symbol? symbol)
    {
        if (!_scope.TryLookup(identifierToken.Text, out symbol))
        {
            _diagnostics.ReportUndefinedName(identifierToken);

            return false;
        }
        return true;
    }

    private bool TryLookupSymbol<T>(Token identifierToken, [NotNullWhen(true)] out T? symbol) where T : notnull, Symbol
    {
        if (!_scope.TryLookup(identifierToken.Text, out symbol, out var existingSymbol))
        {
            if (existingSymbol is not null)
                _diagnostics.ReportInvalidSymbol(identifierToken, Symbol.GetKind<T>(), existingSymbol.Kind);
            else
                _diagnostics.ReportUndefinedName(identifierToken);

            return false;
        }
        return true;
    }

    BoundStatement IStatementVisitor<BoundStatement>.Accept(ForStatement statement)
    {
        var lowerBound = BindExpression(statement.LowerBound, BuiltinTypes.I32);
        var upperBound = BindExpression(statement.UpperBound, BuiltinTypes.I32);

        _scope = new BoundScope(_scope);

        VariableSymbol variable;
        if (statement.DeclaresVariable)
        {
            variable = new VariableSymbol(statement.IdentifierToken.Text, IsReadOnly: true, BuiltinTypes.I32);
            // Check outer scope for name.
            if (_scope.Parent!.TryLookup(variable.Name, out _) || !_scope.TryDeclare(variable))
                _diagnostics.ReportRedeclaration(statement.IdentifierToken);
        }
        else
        {
            if (!TryLookupSymbol(statement.IdentifierToken, out variable!))
            {
                variable = new VariableSymbol(statement.IdentifierToken.Text, IsReadOnly: true, BuiltinTypes.I32);
            }
            else if (variable.IsReadOnly)
            {
                _diagnostics.ReportReadOnlyAssignment(statement.IdentifierToken.Span, statement.IdentifierToken.Text);
            }
            else if (variable.Type != BuiltinTypes.I32)
            {
                _diagnostics.ReportInvalidVariableType(statement.IdentifierToken.Span, BuiltinTypes.I32, variable.Type);
            }
        }

        var body = BindStatement(statement.Body);

        _scope = _scope.Parent!;

        return new BoundForStatement(variable, lowerBound, upperBound, body);
    }

    BoundStatement IStatementVisitor<BoundStatement>.Accept(ExpressionStatement statement)
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


    BoundExpression IExpressionVisitor<BoundExpression>.Visit(IfExpression expression)
    {
        var condition = BindExpression(expression.Condition, BuiltinTypes.Bool);
        var then = BindExpression(expression.Then);
        var @else = BindExpression(expression.Else);

        if (then.Type != @else.Type)
        {

        }

        return new BoundIfExpression(condition, then, @else, then.Type);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(GroupExpression expression) => expression.Expression.Accept(this);

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(UnaryExpression expression)
    {
        var boundOperand = BindExpression(expression.Operand);

        if (boundOperand.Type == BuiltinTypes.Never)
            return new BoundNeverExpression();

        var boundOperator = BoundUnaryOperator.Bind(expression.OperatorToken.Kind, boundOperand.Type);
        if (boundOperator is null)
        {
            _diagnostics.ReportUndefinedUnaryOperator(expression.OperatorToken, boundOperand.Type);
            return new BoundNeverExpression();
        }
        return new BoundUnaryExpression(boundOperator, boundOperand);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(BinaryExpression expression)
    {
        var boundLeft = BindExpression(expression.Left);
        var boundRight = BindExpression(expression.Right);

        if (boundLeft.Type == BuiltinTypes.Never || boundRight.Type == BuiltinTypes.Never)
            return new BoundNeverExpression();

        var resultType = default(TypeSymbol);
        if (expression.OperatorToken.Kind is TokenKind.As)
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
        var boundOperator = BoundBinaryOperator.Bind(expression.OperatorToken.Kind, boundLeft.Type, boundRight.Type, resultType);
        if (boundOperator is null)
        {
            if (expression.OperatorToken.Kind is TokenKind.As)
                _diagnostics.ReportInvalidConversion(TextSpan.FromBounds(expression.OperatorToken.Span.Start, expression.Right.Span.End), boundLeft.Type, resultType!);
            else
                _diagnostics.ReportUndefinedBinaryOperator(expression.OperatorToken, boundLeft.Type, boundRight.Type);
            return new BoundNeverExpression();
        }
        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(LiteralExpression expression)
    {
        var value = expression.Value ?? 0;
        return new BoundLiteralExpression(value);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(NameExpression expression)
    {
        if (expression.IdentifierToken.IsMissing)
        {
            return new BoundNeverExpression();
        }

        if (!TryLookupSymbol(expression.IdentifierToken, out var symbol))
        {
            return new BoundNeverExpression();
        }

        return new BoundSymbolExpression(symbol);
    }
    BoundExpression IExpressionVisitor<BoundExpression>.Visit(CallExpression expression)
    {
        var boundArguments = expression.Arguments.Select(BindExpression).ToArray();

        if (!TryLookupSymbol(expression.IdentifierToken, out FunctionSymbol? function))
        {
            return new BoundNeverExpression();
        }

        if (boundArguments.Length != function.Parameters.Length)
        {
            _diagnostics.ReportInvalidArgumentCount(expression.Span, function.Name, function.Parameters.Length, boundArguments.Length);
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

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(AssignmentExpression expression)
    {
        var boundExpression = BindExpression(expression.Expression);

        if (boundExpression.Type == BuiltinTypes.Never)
            return new BoundNeverExpression();

        if (!TryLookupSymbol(expression.IdentifierToken, out VariableSymbol? variable))
        {
            return boundExpression;
        }

        if (variable.IsReadOnly)
        {
            _diagnostics.ReportReadOnlyAssignment(expression.EqualsToken.Span, expression.IdentifierToken.Text);
        }

        if (boundExpression.Type != variable.Type)
        {
            _diagnostics.ReportInvalidConversion(expression.Span, variable.Type, boundExpression.Type);
            return boundExpression;
        }

        return new BoundAssignmentExpression(variable, boundExpression);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(ConvertExpression expression)
    {
        if (!BuiltinTypes.TryLookup(expression.TypeToken.Text, out var targetType))
        {
            _diagnostics.ReportUndefinedName(expression.TypeToken);
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