using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax, BinderContext context)
    {
        // TODO: Allow non identifier expressions.
        if (syntax.Left is not IdentifierNameExpressionSyntax identifierName)
        {
            throw new NotSupportedException($"Unexpected left hand side of {nameof(AssignmentExpressionSyntax)} '{syntax.Left.SyntaxKind}'");
        }

        var left = BindIdentifierNameExpression(identifierName, context);

        if (left.Type.IsNever)
        {
            return left;
        }

        var symbol = ((BoundIdentifierNameExpression)left).NameSymbol;

        if (symbol.IsReadOnly)
        {
            context.Diagnostics.ReportReadOnlyAssignment(left.Syntax.Location, symbol.Name);
            return new BoundNeverExpression(syntax);
        }

        var right = default(BoundExpression);

        if (syntax.SyntaxKind is SyntaxKind.SimpleAssignmentExpression)
        {
            right = symbol switch
            {
                VariableSymbol _ => BindExpression(syntax.Right, context),
                FunctionSymbol f => BindFunctionBody(syntax.Right, f, context),
                _ => throw new UnreachableException($"Unexpected symbol '{symbol.GetType().Name}'")
            };
        }
        else
        {
            // Note: We don't need to handle functions because none of these operators is not defined for them.
            // TODO: Do this in the lowering step instead?
            var (operatorKind, operatorPrecedence) = SyntaxFacts.GetBinaryOperatorPrecedence(syntax.Operator.SyntaxKind);
            var binaryExpressionKind = SyntaxFacts.GetBinaryOperatorExpression(operatorKind);
            var binaryExpressionSyntax = new BinaryExpressionSyntax(
                binaryExpressionKind,
                syntax.SyntaxTree,
                syntax.Left,
                SyntaxFactory.Operator(operatorKind, syntax.SyntaxTree, operatorPrecedence),
                syntax.Right);

            right = BindBinaryExpression(binaryExpressionSyntax, context);
        }

        right = Coerce(right, left.Type, context);

        if (right.Type.IsNever)
        {
            return right;
        }

        return new BoundAssignmentExpression(syntax, left.Type, left, right);
    }
}
