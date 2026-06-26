namespace CodeAnalysis.Syntax;

public abstract record class ExpressionSyntax(SyntaxKind Kind) : SyntaxNode(Kind);

public sealed record class ModuleExpressionSyntax(SyntaxToken ModuleKeyword) : ExpressionSyntax(SyntaxKind.ModuleExpression)
{
    public override IEnumerable<SyntaxNode> Children() { yield return ModuleKeyword; }
}

public sealed record class TypeExpressionSyntax(
    SyntaxToken TypeKeyword,
    SyntaxToken BraceOpenToken,
    SyntaxList<LocalDeclarationSyntax> Properties,
    SyntaxToken BraceCloseToken)
    : ExpressionSyntax(SyntaxKind.TypeExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return TypeKeyword;
        yield return BraceOpenToken;
        foreach (var property in Properties) yield return property;
        yield return BraceCloseToken;
    }
}

public sealed record class BlockExpressionSyntax(
    SyntaxToken BraceOpenToken,
    SyntaxList<SyntaxNode> Items,
    SyntaxToken BraceCloseToken)
    : ExpressionSyntax(SyntaxKind.BlockExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BraceOpenToken;
        foreach (var item in Items) yield return item;
        yield return BraceCloseToken;
    }
}

public sealed record class IfElseExpressionSyntax(
    SyntaxToken IfKeyword,
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Condition,
    SyntaxToken ParenthesisCloseToken,
    ExpressionSyntax Then,
    ElseClauseSyntax? ElseClause)
    : ExpressionSyntax(SyntaxKind.IfElseExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IfKeyword;
        yield return ParenthesisOpenToken;
        yield return Condition;
        yield return ParenthesisCloseToken;
        yield return Then;
        if (ElseClause is null) yield break;
        yield return ElseClause.ElseKeyword;
        yield return ElseClause.Else;
    }
}

public sealed record class WhileExpressionSyntax(
    SyntaxToken WhileKeyword,
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Condition,
    SyntaxToken ParenthesisCloseToken,
    ExpressionSyntax Body)
    : ExpressionSyntax(SyntaxKind.WhileExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return WhileKeyword;
        yield return ParenthesisOpenToken;
        yield return Condition;
        yield return ParenthesisCloseToken;
        yield return Body;
    }
}

public sealed record class ContinueExpressionSyntax(SyntaxToken ContinueKeyword, ExpressionSyntax? Expression)
    : ExpressionSyntax(SyntaxKind.ContinueExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ContinueKeyword;
        if (Expression is not null) yield return Expression;
    }
}

public sealed record class BreakExpressionSyntax(SyntaxToken BreakKeyword, ExpressionSyntax? Expression)
    : ExpressionSyntax(SyntaxKind.BreakExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BreakKeyword;
        if (Expression is not null) yield return Expression;
    }
}

public sealed record class ReturnExpressionSyntax(SyntaxToken ReturnKeyword, ExpressionSyntax? Expression)
    : ExpressionSyntax(SyntaxKind.ReturnExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ReturnKeyword;
        if (Expression is not null) yield return Expression;
    }
}

public sealed record class ElseClauseSyntax(SyntaxToken ElseKeyword, ExpressionSyntax Else);

public sealed record class ArrayInitializerExpressionSyntax(
    SyntaxToken BracketOpenToken,
    SeparatedSyntaxList<ExpressionSyntax> Elements,
    SyntaxToken BracketCloseToken)
    : ExpressionSyntax(SyntaxKind.ArrayInitializerExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BracketOpenToken;
        foreach (var node in Elements.SyntaxNodes)
            yield return node;
        yield return BracketCloseToken;
    }
}

public sealed record class ObjectInitializerExpressionSyntax(
    ExpressionSyntax TypeName,
    SyntaxToken BraceOpenToken,
    SeparatedSyntaxList<PropertyInitializerExpressionSyntax> Properties,
    SyntaxToken BraceCloseToken)
    : ExpressionSyntax(SyntaxKind.ObjectInitializerExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return TypeName;
        yield return BraceOpenToken;
        foreach (var property in Properties) yield return property;
        yield return BraceCloseToken;
    }
}

public sealed record class PropertyInitializerExpressionSyntax(SimpleNameSyntax PropertyName, SyntaxToken EqualsToken, ExpressionSyntax Value)
    : SyntaxNode(SyntaxKind.PropertyInitializerExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return PropertyName;
        yield return EqualsToken;
        yield return Value;
    }
}

public sealed record class ElementAccessExpressionSyntax(
    ExpressionSyntax Receiver,
    SyntaxToken BracketOpen,
    ExpressionSyntax Index,
    SyntaxToken BracketClose)
    : ExpressionSyntax(SyntaxKind.ElementAccessExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Receiver;
        yield return BracketOpen;
        yield return Index;
        yield return BracketClose;
    }
}

public sealed record class MemberAccessExpressionSyntax(
    ExpressionSyntax Receiver,
    SyntaxToken DotToken,
    SimpleNameSyntax MemberName)
    : ExpressionSyntax(SyntaxKind.MemberAccessExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Receiver;
        yield return DotToken;
        yield return MemberName;
    }
}

public sealed record class ConversionExpressionSyntax(
    ExpressionSyntax Expression,
    SyntaxToken AsKeyword,
    TypeSyntax Type)
    : ExpressionSyntax(SyntaxKind.ConversionExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return AsKeyword;
        yield return Type;
    }
}

public sealed record class LambdaExpressionSyntax(
    SyntaxToken ParenthesisOpenToken,
    SeparatedSyntaxList<SimpleNameSyntax> Parameters,
    SyntaxToken ParenthesisCloseToken,
    SyntaxToken EqualsGreaterThanToken,
    ExpressionSyntax Body)
    : ExpressionSyntax(SyntaxKind.LambdaExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ParenthesisOpenToken;
        foreach (var syntax in Parameters.SyntaxNodes) yield return syntax;
        yield return ParenthesisCloseToken;
        yield return EqualsGreaterThanToken;
        yield return Body;
    }
}

public sealed record class InvocationExpressionSyntax(
    ExpressionSyntax Callee,
    SyntaxToken ParenthesisOpenToken,
    SeparatedSyntaxList<ExpressionSyntax> Arguments,
    SyntaxToken ParenthesisCloseToken)
    : ExpressionSyntax(SyntaxKind.InvocationExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Callee;
        yield return ParenthesisOpenToken;
        foreach (var argument in Arguments) yield return argument;
        yield return ParenthesisCloseToken;
    }
}

public sealed record class GroupExpressionSyntax(
    SyntaxToken ParenthesisOpenToken,
    ExpressionSyntax Expression,
    SyntaxToken ParenthesisCloseToken)
    : ExpressionSyntax(SyntaxKind.GroupExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ParenthesisOpenToken;
        yield return Expression;
        yield return ParenthesisCloseToken;
    }
}

public sealed record class UnaryExpressionSyntax(SyntaxToken OperatorToken, ExpressionSyntax Operand)
    : ExpressionSyntax(SyntaxFacts.GetUnaryOperatorExpression(OperatorToken.Kind))
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return OperatorToken;
        yield return Operand;
    }
}

public sealed record class BinaryExpressionSyntax(ExpressionSyntax Left, SyntaxToken OperatorToken, ExpressionSyntax Right)
    : ExpressionSyntax(SyntaxFacts.GetBinaryOperatorExpression(OperatorToken.Kind))
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}

public sealed record class AssignmentExpressionSyntax(ExpressionSyntax Left, SyntaxToken EqualsToken, ExpressionSyntax Right)
    : ExpressionSyntax(SyntaxKind.AssignmentExpression)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return EqualsToken;
        yield return Right;
    }
}

public sealed record class NameExpressionSyntax(NameSyntax Name) : ExpressionSyntax(SyntaxKind.NameExpression)
{
    public override IEnumerable<SyntaxNode> Children() { yield return Name; }
}

public sealed record class LiteralExpressionSyntax(SyntaxKind Kind, SyntaxToken LiteralToken, object InstanceValue)
    : ExpressionSyntax(Kind)
{
    public override IEnumerable<SyntaxNode> Children() { yield return LiteralToken; }
}
