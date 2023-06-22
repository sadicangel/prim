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
        var parentScope = CreateParentScopes(previousScope);
        var binder = new Binder(parentScope);
        var expression = binder.BindStatement(compilationUnit.Statement);
        var variables = binder._scope.Variables;
        var diagnostics = binder.Diagnostics;

        return new BoundGlobalScope(diagnostics, variables, expression, previousScope);
    }

    private static BoundScope? CreateParentScopes(BoundGlobalScope? previous)
    {
        var stack = new Stack<BoundGlobalScope>();

        while (previous is not null)
        {
            stack.Push(previous);
            previous = previous.Previous;
        }

        var parent = default(BoundScope);

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
        var isReadOnly = statement.KeywordToken.Kind == TokenKind.Const;
        var expression = BindExpression(statement.Expression);
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
            if (_scope.Parent!.TryLookup(variable.Name, out _) || !_scope.TryDeclare(variable))
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
        var expression = BindExpression(statement.Expression);
        return new BoundExpressionStatement(expression);
    }

    private BoundExpression BindExpression(Expression expression, TypeSymbol targetType)
    {
        var boundExpression = BindExpression(expression);

        if (boundExpression.Type != TypeSymbol.Never && targetType != TypeSymbol.Never && boundExpression.Type != targetType)
            _diagnostics.ReportInvalidConversion(expression.Span, boundExpression.Type, targetType);
        return boundExpression;
    }

    private BoundExpression BindExpression(Expression expression) => expression.Accept(this);


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

        if (!_scope.TryLookup(name, out var variable))
        {
            _diagnostics.ReportUndefinedName(expression.IdentifierToken);
            return new BoundNeverExpression();
        }

        return new BoundVariableExpression(variable);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(AssignmentExpression expression)
    {
        var name = expression.IdentifierToken.Text;
        var boundExpression = BindExpression(expression.Expression);

        if (boundExpression.Type == TypeSymbol.Never)
            return new BoundNeverExpression();

        if (!_scope.TryLookup(name, out var variable))
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
}