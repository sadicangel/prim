using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

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
                throw new InvalidOperationException($"Could not declare builtin function {function.Name}");
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
            if (TypeSymbol.GetTypeSymbol(statement.TypeToken.Text) is TypeSymbol type)
            {
                expression = BindExpression(statement.Expression, type);
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
        var condition = BindExpression(statement.Condition, TypeSymbol.Bool);
        var then = BindStatement(statement.Then);
        var @else = statement.HasElseClause ? BindStatement(statement.Else) : null;
        return new BoundIfStatement(condition, then, @else);
    }

    BoundStatement IStatementVisitor<BoundStatement>.Accept(WhileStatement statement)
    {
        var condition = BindExpression(statement.Condition, TypeSymbol.Bool);
        var body = BindStatement(statement.Body);
        return new BoundWhileStatement(condition, body);
    }

    BoundStatement IStatementVisitor<BoundStatement>.Accept(ForStatement statement)
    {
        var lowerBound = BindExpression(statement.LowerBound, TypeSymbol.I32);
        var upperBound = BindExpression(statement.UpperBound, TypeSymbol.I32);

        _scope = new BoundScope(_scope);

        VariableSymbol variable;
        if (statement.DeclaresVariable)
        {
            variable = new VariableSymbol(statement.IdentifierToken.Text, IsReadOnly: true, TypeSymbol.I32);
            // Check outer scope for name.
            if (_scope.Parent!.TryLookup(variable.Name, out VariableSymbol? _) || !_scope.TryDeclare(variable))
                _diagnostics.ReportRedeclaration(statement.IdentifierToken);
        }
        else
        {
            if (!_scope.TryLookup(statement.IdentifierToken.Text, out variable!))
            {
                variable = new VariableSymbol(statement.IdentifierToken.Text, IsReadOnly: true, TypeSymbol.I32);
                _diagnostics.ReportUndefinedName(statement.IdentifierToken);
            }
            else if (variable.IsReadOnly)
            {
                _diagnostics.ReportReadOnlyAssignment(statement.IdentifierToken.Span, statement.IdentifierToken.Text);
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

    private BoundExpression BindExpression(Expression expression, TypeSymbol targetType)
    {
        var boundExpression = BindExpression(expression);

        var conversion = Conversion.Classify(boundExpression.Type, targetType);

        if (conversion.Exists)
        {
            if (!conversion.IsIdentity)
            {
                if (conversion.IsImplicit)
                    boundExpression = new BoundConvertExpression(boundExpression, targetType);
                else
                    _diagnostics.ReportInvalidImplicitConversion(expression.Span, boundExpression.Type, targetType);
            }
        }
        else
        {
            _diagnostics.ReportInvalidConversion(expression.Span, boundExpression.Type, targetType);
        }

        return boundExpression;
    }

    private BoundExpression BindExpression(Expression expression) => BindExpression(expression, allowVoid: false);

    private BoundExpression BindExpression(Expression expression, bool allowVoid)
    {
        var result = expression.Accept(this);
        if (!allowVoid && result.Type == TypeSymbol.Void)
        {
            _diagnostics.ReportInvalidExpressionType(expression.Span, result.Type);
            return new BoundNeverExpression();
        }
        return result;
    }


    BoundExpression IExpressionVisitor<BoundExpression>.Visit(IfExpression expression)
    {
        var condition = BindExpression(expression.Condition, TypeSymbol.Bool);
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

        if (boundOperand.Type == TypeSymbol.Never)
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

        if (boundLeft.Type == TypeSymbol.Never || boundRight.Type == TypeSymbol.Never)
            return new BoundNeverExpression();

        var boundOperator = BoundBinaryOperator.Bind(expression.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
        if (boundOperator is null)
        {
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
        var name = expression.IdentifierToken.Text;

        if (expression.IdentifierToken.IsMissing)
        {
            return new BoundNeverExpression();
        }

        if (!_scope.TryLookup(name, out VariableSymbol? variable))
        {
            _diagnostics.ReportUndefinedName(expression.IdentifierToken);
            return new BoundNeverExpression();
        }

        return new BoundVariableExpression(variable);
    }
    BoundExpression IExpressionVisitor<BoundExpression>.Visit(CallExpression expression)
    {
        var boundArguments = expression.Arguments.Select(BindExpression).ToArray();

        if (!_scope.TryLookup(expression.Identifier.Text, out FunctionSymbol? function))
        {
            _diagnostics.ReportUndefinedName(expression.Identifier);
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
        var name = expression.IdentifierToken.Text;
        var boundExpression = BindExpression(expression.Expression);

        if (boundExpression.Type == TypeSymbol.Never)
            return new BoundNeverExpression();

        if (!_scope.TryLookup(name, out VariableSymbol? variable))
        {
            _diagnostics.ReportUndefinedName(expression.IdentifierToken);
            return boundExpression;
        }

        if (variable.IsReadOnly)
        {
            _diagnostics.ReportReadOnlyAssignment(expression.EqualsToken.Span, name);
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
        var boundExpression = BindExpression(expression.Expression);
        if (TypeSymbol.GetTypeSymbol(expression.TypeToken.Text) is not TypeSymbol type)
        {
            _diagnostics.ReportUndefinedName(expression.TypeToken);
            return new BoundNeverExpression();
        }
        var conversion = Conversion.Classify(boundExpression.Type, type);
        if (!conversion.Exists)
        {
            _diagnostics.ReportInvalidConversion(expression.AsToken.Span, boundExpression.Type, type);
            return new BoundNeverExpression();
        }
        return new BoundConvertExpression(boundExpression, type);
    }
}