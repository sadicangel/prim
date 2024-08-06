using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Expressions.ControlFlow;
using CodeAnalysis.Syntax.Expressions.Declarations;
using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindExpression(ExpressionSyntax syntax, Context context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.SimpleName =>
                BindSimpleName((SimpleNameSyntax)syntax, context),
            SyntaxKind.QualifiedName =>
                BindQualifiedName((QualifiedNameSyntax)syntax, context),
            >= SyntaxKind.I8LiteralExpression and <= SyntaxKind.NullLiteralExpression =>
                BindLiteralExpression((LiteralExpressionSyntax)syntax, context),
            SyntaxKind.GroupExpression =>
                BindGroupExpression((GroupExpressionSyntax)syntax, context),
            SyntaxKind.AssignmentExpression =>
                BindAssignmentExpression((AssignmentExpressionSyntax)syntax, context),
            SyntaxKind.InitValueExpression =>
                BindInitValueExpression((InitValueExpressionSyntax)syntax, context),
            SyntaxKind.VariableDeclaration =>
                BindVariableDeclaration((VariableDeclarationSyntax)syntax, context),
            SyntaxKind.ModuleDeclaration =>
                BindModuleDeclaration((ModuleDeclarationSyntax)syntax, context),
            SyntaxKind.StructDeclaration =>
                BindStructDeclaration((StructDeclarationSyntax)syntax, context),
            SyntaxKind.LocalDeclaration =>
                BindLocalDeclaration((LocalDeclarationSyntax)syntax, context),
            SyntaxKind.EmptyExpression =>
                BindEmptyExpression((EmptyExpressionSyntax)syntax, context),
            SyntaxKind.StatementExpression =>
                BindStatementExpression((StatementExpressionSyntax)syntax, context),
            SyntaxKind.BlockExpression =>
                BindBlockExpression((BlockExpressionSyntax)syntax, context),
            SyntaxKind.ArrayExpression =>
                BindArrayInitExpression((ArrayInitExpressionSyntax)syntax, context),
            SyntaxKind.StructInitExpression =>
                BindStructInitExpression((StructInitExpressionSyntax)syntax, context),
            SyntaxKind.MemberAccessExpression =>
                BindMemberAccessExpression((MemberAccessExpressionSyntax)syntax, context),
            SyntaxKind.IndexExpression =>
                BindIndexExpression((IndexExpressionSyntax)syntax, context),
            SyntaxKind.InvocationExpression =>
                BindInvocationExpression((InvocationExpressionSyntax)syntax, context),
            SyntaxKind.ConversionExpression =>
                BindConversionExpression((ConversionExpressionSyntax)syntax, context),
            SyntaxKind.UnaryPlusExpression or
            SyntaxKind.UnaryMinusExpression or
            SyntaxKind.OnesComplementExpression or
            SyntaxKind.NotExpression =>
                BindUnaryExpression((UnaryExpressionSyntax)syntax, context),
            SyntaxKind.AddExpression or
            SyntaxKind.SubtractExpression or
            SyntaxKind.MultiplyExpression or
            SyntaxKind.DivideExpression or
            SyntaxKind.ModuloExpression or
            SyntaxKind.PowerExpression or
            SyntaxKind.LeftShiftExpression or
            SyntaxKind.RightShiftExpression or
            SyntaxKind.LogicalOrExpression or
            SyntaxKind.LogicalAndExpression or
            SyntaxKind.BitwiseOrExpression or
            SyntaxKind.BitwiseAndExpression or
            SyntaxKind.ExclusiveOrExpression or
            SyntaxKind.EqualsExpression or
            SyntaxKind.NotEqualsExpression or
            SyntaxKind.LessThanExpression or
            SyntaxKind.LessThanOrEqualExpression or
            SyntaxKind.GreaterThanExpression or
            SyntaxKind.GreaterThanOrEqualExpression or
            SyntaxKind.CoalesceExpression =>
                BindBinaryExpression((BinaryExpressionSyntax)syntax, context),
            SyntaxKind.IfExpression =>
                BindIfElseExpression((IfExpressionSyntax)syntax, context),
            SyntaxKind.WhileExpression =>
                BindWhileExpression((WhileExpressionSyntax)syntax, context),
            SyntaxKind.ContinueExpression =>
                BindContinueExpression((ContinueExpressionSyntax)syntax, context),
            SyntaxKind.BreakExpression =>
                BindBreakExpression((BreakExpressionSyntax)syntax, context),
            SyntaxKind.ReturnExpression =>
                BindReturnExpression((ReturnExpressionSyntax)syntax, context),
            _ =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
        };
    }
}
