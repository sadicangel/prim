using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindExpression(ExpressionSyntax syntax, BindingContext context)
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
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.SimpleAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.AddAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.SubtractAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.MultiplyAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.DivideAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.ModuloAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.PowerAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.AndAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.ExclusiveOrAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.OrAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.LeftShiftAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.RightShiftAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.CoalesceAssignmentExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.VariableDeclaration =>
                BindVariableDeclaration((VariableDeclarationSyntax)syntax, context),
            SyntaxKind.FunctionDeclaration =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.StructDeclaration =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.PropertyDeclaration =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.LocalDeclaration =>
                BindLocalDeclaration((LocalDeclarationSyntax)syntax, context),
            SyntaxKind.EmptyExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.StatementExpression =>
                BindStatementExpression((StatementExpressionSyntax)syntax, context),
            SyntaxKind.BlockExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.ArrayExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.StructExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.PropertyExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.UnaryPlusExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.UnaryMinusExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.PrefixIncrementExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.PrefixDecrementExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.OnesComplementExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.NotExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.AddExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.SubtractExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.MultiplyExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.DivideExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.ModuloExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.PowerExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.LeftShiftExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.RightShiftExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.LogicalOrExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.LogicalAndExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.BitwiseOrExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.BitwiseAndExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.ExclusiveOrExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.EqualsExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.NotEqualsExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.LessThanExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.LessThanOrEqualExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.GreaterThanExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.GreaterThanOrEqualExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            SyntaxKind.CoalesceExpression =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
            _ =>
                throw new NotImplementedException(syntax.SyntaxKind.ToString()),
        };
    }
}
