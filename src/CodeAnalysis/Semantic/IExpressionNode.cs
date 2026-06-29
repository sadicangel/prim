using System.Collections.Immutable;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal interface IExpressionNode : INode
{
    TypeSymbol Type { get; }
}

internal interface IReferenceNode : IExpressionNode
{
    Symbol Symbol { get; }

    public bool IsReadOnly => Symbol.IsReadOnly;

    public bool IsStatic => Symbol.IsStatic;
}

internal sealed record class TypeReferenceNode(SyntaxNode Syntax, TypeSymbol Symbol) : IReferenceNode
{
    public NodeKind Kind => NodeKind.TypeReference;

    public TypeSymbol Type => Symbol.ContainingModule.TypeType;

    Symbol IReferenceNode.Symbol => Symbol;
}

internal sealed record class ModuleReferenceNode(SyntaxNode Syntax, ModuleSymbol Symbol) : IReferenceNode
{
    public NodeKind Kind => NodeKind.ModuleReference;

    public TypeSymbol Type => Symbol.ModuleType;

    Symbol IReferenceNode.Symbol => Symbol;
}

internal sealed record class VariableReferenceNode(SyntaxNode Syntax, VariableSymbol Symbol) : IReferenceNode
{
    public NodeKind Kind => NodeKind.VariableReference;

    public TypeSymbol Type => Symbol.Type;

    Symbol IReferenceNode.Symbol => Symbol;
}

internal sealed record class ElementReferenceNode(SyntaxNode Syntax, IndexerSymbol Symbol, IExpressionNode Receiver, IExpressionNode Index) : IReferenceNode
{
    public NodeKind Kind => NodeKind.ElementReference;

    public TypeSymbol Type => Symbol.Type;

    Symbol IReferenceNode.Symbol => Symbol;
}

internal sealed record class MemberReferenceNode(SyntaxNode Syntax, MemberSymbol Symbol, IExpressionNode Receiver) : IReferenceNode
{
    public NodeKind Kind => NodeKind.MemberReference;

    public TypeSymbol Type => Symbol.Type;

    Symbol IReferenceNode.Symbol => Symbol;
}

internal sealed record class LiteralExpressionNode(SyntaxNode Syntax, TypeSymbol Type, object Value) : IExpressionNode
{
    public NodeKind Kind => NodeKind.LiteralExpression;
}

internal sealed record class LambdaExpressionNode(SyntaxNode Syntax, LambdaTypeSymbol Type, ImmutableArray<VariableSymbol> Parameters, IExpressionNode Body) : IExpressionNode
{
    public NodeKind Kind => NodeKind.LambdaExpression;
    TypeSymbol IExpressionNode.Type => Type;
}

internal sealed record class BlockExpressionNode(SyntaxNode Syntax, TypeSymbol Type, ImmutableArray<INode> Items) : IExpressionNode
{
    public NodeKind Kind => NodeKind.BlockExpression;
}

internal sealed record class ArrayInitializerExpressionNode(SyntaxNode Syntax, TypeSymbol Type, ImmutableArray<IExpressionNode> Elements) : IExpressionNode
{
    public NodeKind Kind => NodeKind.ArrayInitializerExpression;
}

internal sealed record class InvocationExpressionNode(SyntaxNode Syntax, TypeSymbol Type, IExpressionNode Callee, ImmutableArray<IExpressionNode> Arguments) : IExpressionNode
{
    public NodeKind Kind => NodeKind.InvocationExpression;
}

internal sealed record class ModuleInitializerExpressionNode(SyntaxNode Syntax, TypeSymbol Type) : IExpressionNode
{
    public NodeKind Kind => NodeKind.ModuleInitializerExpression;
}

internal sealed record class TypeInitializerExpressionNode(SyntaxNode Syntax, TypeSymbol Type, ImmutableArray<DeclarationNode> Properties) : IExpressionNode
{
    public NodeKind Kind => NodeKind.TypeInitializerExpression;
}

internal sealed record class ObjectInitializerExpressionNode(SyntaxNode Syntax, TypeReferenceNode TypeReference, ImmutableArray<PropertyInitializerExpressionNode> PropertyInitializers) : IExpressionNode
{
    public NodeKind Kind => NodeKind.ObjectInitializerExpression;
    public TypeSymbol Type => TypeReference.Symbol;
}

internal sealed record class PropertyInitializerExpressionNode(SyntaxNode Syntax, PropertySymbol Property, IExpressionNode Value) : IExpressionNode // TODO: Probably does not need to be expression.
{
    public NodeKind Kind => NodeKind.PropertyInitializerExpression;
    public TypeSymbol Type => Property.Type;
}

internal sealed record class AssignmentExpressionNode(SyntaxNode Syntax, IReferenceNode Reference, IExpressionNode Value) : IExpressionNode
{
    public NodeKind Kind => NodeKind.AssignmentExpression;
    public TypeSymbol Type => Reference.Type;
}

internal sealed record class UnaryExpressionNode(SyntaxNode Syntax, OperatorSymbol Operator, IExpressionNode Operand) : IExpressionNode
{
    public NodeKind Kind => NodeKind.UnaryExpression;
    public TypeSymbol Type => Operator.Type;
}

internal sealed record class BinaryExpressionNode(SyntaxNode Syntax, IExpressionNode Left, OperatorSymbol Operator, IExpressionNode Right) : IExpressionNode
{
    public NodeKind Kind => NodeKind.BinaryExpression;
    public TypeSymbol Type => Operator.Type;
}

internal sealed record class IfElseExpressionNode(SyntaxNode Syntax, TypeSymbol Type, IExpressionNode Condition, IExpressionNode Then, IExpressionNode? Else) : IExpressionNode
{
    public NodeKind Kind => NodeKind.IfElseExpression;
}

internal sealed record class WhileExpressionNode(SyntaxNode Syntax, IExpressionNode Condition, IExpressionNode Body) : IExpressionNode
{
    public NodeKind Kind => NodeKind.WhileExpression;
    public TypeSymbol Type => Body.Type;
}

internal sealed record class ContinueExpressionNode(SyntaxNode Syntax, IExpressionNode Expression) : IExpressionNode
{
    public NodeKind Kind => NodeKind.ContinueExpression;
    public TypeSymbol Type => Expression.Type;
}

internal sealed record class BreakExpressionNode(SyntaxNode Syntax, IExpressionNode Expression) : IExpressionNode
{
    public NodeKind Kind => NodeKind.BreakExpression;
    public TypeSymbol Type => Expression.Type;
}

internal sealed record class ReturnExpressionNode(SyntaxNode Syntax, IExpressionNode Expression) : IExpressionNode
{
    public NodeKind Kind => NodeKind.ReturnExpression;
    public TypeSymbol Type => Expression.Type;
}

internal sealed record class NopExpressionNode(SyntaxNode Syntax, TypeSymbol Type) : IExpressionNode
{
    public NodeKind Kind => NodeKind.NopExpression;
}

internal sealed record class NeverExpressionNode(SyntaxNode Syntax, TypeSymbol Type) : IExpressionNode
{
    public NodeKind Kind => NodeKind.NeverExpression;
}
