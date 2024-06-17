using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax, BinderContext context)
    {
        if (syntax.SyntaxKind is SyntaxKind.SimpleAssignmentExpression)
        {
            var left = BindExpression(syntax.Left, context);
            var right = BindExpression(syntax.Right, context);

            return new BoundAssignmentExpression(syntax, left.Type, left, right);
        }
        else
        {
            var (operatorKind, operatorPrecedence) = SyntaxFacts.GetBinaryOperatorPrecedence(syntax.Operator.SyntaxKind);
            var operatorToken = SyntaxFactory.Token(operatorKind, syntax.SyntaxTree);
            var binaryExpressionKind = SyntaxFacts.GetBinaryOperatorExpression(operatorKind);
            var binaryExpressionSyntax = new BinaryExpressionSyntax(
                binaryExpressionKind,
                syntax.SyntaxTree,
                syntax.Left,
                SyntaxFactory.Operator(operatorKind, syntax.SyntaxTree, operatorPrecedence),
                syntax.Right);

            var right = BindBinaryExpression(binaryExpressionSyntax, context);
            if (right is not BoundBinaryExpression binaryExpression)
                return right;
            var left = binaryExpression.Left;

            return new BoundAssignmentExpression(syntax, left.Type, left, right);
        }
    }
}
