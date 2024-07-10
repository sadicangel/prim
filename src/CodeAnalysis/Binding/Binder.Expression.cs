using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindExpression(ExpressionSyntax syntax, BinderContext context)
    {
        return syntax.SyntaxKind switch
        {
            SyntaxKind.IdentifierNameExpression =>
                BindIdentifierNameExpression((IdentifierNameExpressionSyntax)syntax, context),
            SyntaxKind.I32LiteralExpression or
            SyntaxKind.U32LiteralExpression or
            SyntaxKind.I64LiteralExpression or
            SyntaxKind.U64LiteralExpression or
            SyntaxKind.F32LiteralExpression or
            SyntaxKind.F64LiteralExpression or
            SyntaxKind.StrLiteralExpression or
            SyntaxKind.TrueLiteralExpression or
            SyntaxKind.FalseLiteralExpression or
            SyntaxKind.NullLiteralExpression =>
                BindLiteralExpression((LiteralExpressionSyntax)syntax, context),
            SyntaxKind.GroupExpression =>
                BindGroupExpression((GroupExpressionSyntax)syntax, context),
            SyntaxKind.AssignmentExpression =>
                BindAssignmentExpression((AssignmentExpressionSyntax)syntax, context),
            SyntaxKind.VariableDeclaration =>
                BindVariableDeclaration((VariableDeclarationSyntax)syntax, context),
            SyntaxKind.FunctionDeclaration =>
                BindFunctionDeclaration((FunctionDeclarationSyntax)syntax, context),
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
            _ =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
        };
    }
}
