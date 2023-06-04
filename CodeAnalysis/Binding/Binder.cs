﻿using CodeAnalysis.Syntax;

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
        var isReadOnly = statement.KeywordToken.Kind == TokenKind.Let;
        var expression = BindExpression(statement.Expression);
        var variable = new Variable(name, isReadOnly, expression.Type);

        if (!_scope.TryDeclare(variable))
        {
            _diagnostics.ReportVariableRedeclaration(statement.IdentifierToken);
        }

        return new BoundDeclarationStatement(variable, expression);
    }

    BoundStatement IStatementVisitor<BoundStatement>.Accept(ExpressionStatement statement)
    {
        var expression = BindExpression(statement.Expression);
        return new BoundExpressionStatement(expression);
    }

    private BoundExpression BindExpression(Expression expression) => expression.Accept(this);


    BoundExpression IExpressionVisitor<BoundExpression>.Visit(GroupExpression expression) => expression.Expression.Accept(this);

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(UnaryExpression expression)
    {
        var boundOperand = BindExpression(expression.Operand);
        var boundOperator = BoundUnaryOperator.Bind(expression.OperatorToken.Kind, boundOperand.Type);
        if (boundOperator is null)
        {
            _diagnostics.ReportUndefinedUnaryOperator(expression.OperatorToken, boundOperand.Type);
            return boundOperand;
        }
        return new BoundUnaryExpression(boundOperator, boundOperand);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(BinaryExpression expression)
    {
        var boundLeft = BindExpression(expression.Left);
        var boundRight = BindExpression(expression.Right);
        var boundOperator = BoundBinaryOperator.Bind(expression.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
        if (boundOperator is null)
        {
            _diagnostics.ReportUndefinedBinaryOperator(expression.OperatorToken, boundLeft.Type, boundRight.Type);
            return boundLeft;
        }
        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(LiteralExpression expression)
    {
        var value = expression.Value ?? 0L;
        return new BoundLiteralExpression(value);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(NameExpression expression)
    {
        if (!_scope.TryLookup(expression.IdentifierToken.Text, out var variable))
        {
            _diagnostics.ReportUndefinedName(expression.IdentifierToken);
            return new BoundLiteralExpression(0L);
        }

        return new BoundVariableExpression(variable);
    }

    BoundExpression IExpressionVisitor<BoundExpression>.Visit(AssignmentExpression expression)
    {
        var name = expression.IdentifierToken.Text;
        var boundExpression = BindExpression(expression.Expression);

        if (!_scope.TryLookup(name, out var variable))
        {
            _diagnostics.ReportUndefinedName(expression.IdentifierToken);
            return boundExpression;
        }

        if (variable.IsReadOnly)
        {
            _diagnostics.ReportAssignmentToReadOnlyVariable(expression.EqualsToken.Span, name);
        }

        if (boundExpression.Type != variable.Type)
        {
            _diagnostics.ReportInvalidConversion(expression.Span, boundExpression.Type, variable.Type);
            return boundExpression;
        }

        return new BoundAssignmentExpression(variable, boundExpression);
    }
}