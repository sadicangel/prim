using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Statements;

namespace CodeAnalysis.Binding;

internal abstract class BoundTreeRewriter : IBoundStatementVisitor<BoundStatement>, IBoundExpressionVisitor<BoundExpression>
{
    public BoundStatement Rewrite(BoundStatement statement) => statement.Accept(this);
    protected virtual BoundStatement Rewrite(BoundVariableDeclaration node)
    {
        var expression = Rewrite(node.Expression);
        if (expression == node.Expression)
            return node;
        return new BoundVariableDeclaration(node.Variable, expression);
    }

    protected virtual BoundStatement Rewrite(BoundFunctionDeclaration node)
    {
        var body = Rewrite(node.Body);
        if (body == node.Body)
            return node;
        return new BoundFunctionDeclaration(node.Function, body);
    }

    protected virtual BoundStatement Rewrite(BoundBlockStatement node)
    {
        List<BoundStatement>? statements = null;
        for (var i = 0; i < node.Statements.Count; ++i)
        {
            var oldStatement = node.Statements[i];
            var newStatement = Rewrite(oldStatement);
            if (newStatement != oldStatement && statements is null)
            {
                statements = new List<BoundStatement>(node.Statements.Count);
                for (var j = 0; j < i; ++j)
                    statements.Add(node.Statements[j]);
            }

            if (statements is not null)
            {
                statements.Add(newStatement);
            }
        }
        if (statements is null)
            return node;

        return new BoundBlockStatement(statements);
    }

    protected virtual BoundStatement Rewrite(BoundExpressionStatement node)
    {
        var expression = Rewrite(node.Expression);
        if (expression == node.Expression)
            return node;
        return new BoundExpressionStatement(expression);
    }

    protected virtual BoundStatement Rewrite(BoundIfStatement node)
    {
        var condition = Rewrite(node.Condition);
        var then = Rewrite(node.Then);
        var @else = node.Else is not null ? Rewrite(node.Else) : null;
        if (condition == node.Condition && then == node.Then && @else == node.Else)
            return node;
        return new BoundIfStatement(condition, then, @else);
    }

    protected virtual BoundStatement Rewrite(BoundWhileStatement node)
    {
        var condition = Rewrite(node.Condition);
        var body = Rewrite(node.Body);
        if (condition == node.Condition && body == node.Body)
            return node;
        return new BoundWhileStatement(condition, body, node.Break, node.Continue);
    }

    protected virtual BoundStatement Rewrite(BoundForStatement node)
    {
        var lowerBound = Rewrite(node.LowerBound);
        var upperBound = Rewrite(node.UpperBound);
        var body = Rewrite(node.Body);
        if (lowerBound == upperBound && upperBound == node.UpperBound && body == node.Body)
            return node;
        return new BoundForStatement(node.Variable, lowerBound, upperBound, body, node.Break, node.Continue);
    }

    protected virtual BoundStatement Rewrite(BoundLabelDeclaration node) => node;
    protected virtual BoundStatement Rewrite(BoundGotoStatement node) => node;
    protected virtual BoundStatement Rewrite(BoundConditionalGotoStatement node)
    {
        var condition = Rewrite(node.Condition);
        if (condition == node.Condition)
            return node;
        return new BoundConditionalGotoStatement(node.Label, condition, node.JumpIfTrue);
    }
    protected virtual BoundStatement Rewrite(BoundReturnStatement node)
    {
        var expression = node.Expression is null ? null : Rewrite(node.Expression);
        if (expression == node.Expression)
            return node;
        return new BoundReturnStatement(expression);
    }
    public BoundExpression Rewrite(BoundExpression expression) => expression.Accept(this);
    protected virtual BoundExpression Rewrite(BoundNeverExpression node) => node;
    protected virtual BoundExpression Rewrite(BoundUnaryExpression node)
    {
        var expression = Rewrite(node.Operand);
        if (expression == node.Operand)
            return node;
        return new BoundUnaryExpression(node.Operator, expression);
    }
    protected virtual BoundExpression Rewrite(BoundBinaryExpression node)
    {
        var left = Rewrite(node.Left);
        var right = Rewrite(node.Right);
        if (left == node.Left && right == node.Right)
            return node;
        return new BoundBinaryExpression(left, node.Operator, right);
    }
    protected virtual BoundExpression Rewrite(BoundLiteralExpression node) => node;
    protected virtual BoundExpression Rewrite(BoundSymbolExpression node) => node;
    protected virtual BoundExpression Rewrite(BoundAssignmentExpression node)
    {
        var expression = Rewrite(node.Expression);
        if (expression == node.Expression)
            return node;
        return new BoundAssignmentExpression(node.Variable, expression);
    }

    protected virtual BoundExpression Rewrite(BoundIfExpression node)
    {
        var condition = Rewrite(node.Condition);
        var then = Rewrite(node.Then);
        var @else = Rewrite(node.Else);
        if (condition == node.Condition && then == node.Then && @else == node.Else)
            return node;
        return new BoundIfExpression(condition, then, @else, node.Type);
    }

    protected virtual BoundExpression Rewrite(BoundCallExpression node) => node;
    protected virtual BoundExpression Rewrite(BoundConvertExpression node) => node;

    #region Visitor
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundVariableDeclaration statement) => Rewrite(statement);
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundFunctionDeclaration statement) => Rewrite(statement);
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundBlockStatement statement) => Rewrite(statement);
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundExpressionStatement statement) => Rewrite(statement);
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundIfStatement statement) => Rewrite(statement);
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundWhileStatement statement) => Rewrite(statement);
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundForStatement statement) => Rewrite(statement);

    BoundExpression IBoundExpressionVisitor<BoundExpression>.Visit(BoundNeverExpression expression) => Rewrite(expression);
    BoundExpression IBoundExpressionVisitor<BoundExpression>.Visit(BoundUnaryExpression expression) => Rewrite(expression);
    BoundExpression IBoundExpressionVisitor<BoundExpression>.Visit(BoundBinaryExpression expression) => Rewrite(expression);
    BoundExpression IBoundExpressionVisitor<BoundExpression>.Visit(BoundLiteralExpression expression) => Rewrite(expression);
    BoundExpression IBoundExpressionVisitor<BoundExpression>.Visit(BoundSymbolExpression expression) => Rewrite(expression);
    BoundExpression IBoundExpressionVisitor<BoundExpression>.Visit(BoundAssignmentExpression expression) => Rewrite(expression);
    BoundExpression IBoundExpressionVisitor<BoundExpression>.Visit(BoundIfExpression expression) => Rewrite(expression);
    BoundExpression IBoundExpressionVisitor<BoundExpression>.Visit(BoundCallExpression expression) => Rewrite(expression);
    BoundExpression IBoundExpressionVisitor<BoundExpression>.Visit(BoundConvertExpression expression) => Rewrite(expression);
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundLabelDeclaration statement) => Rewrite(statement);
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundGotoStatement statement) => Rewrite(statement);
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundConditionalGotoStatement statement) => Rewrite(statement);
    BoundStatement IBoundStatementVisitor<BoundStatement>.Visit(BoundReturnStatement statement) => Rewrite(statement);
    #endregion
}
