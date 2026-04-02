using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Expressions;

namespace CodeAnalysis.Semantic;

internal interface IBoundNodeVisitor<out T>
{
    // Declarations
    T Visit(BoundModuleDeclaration node);
    T Visit(BoundPredefinedDeclaration node);
    T Visit(BoundPropertyDeclaration node);
    T Visit(BoundStructDeclaration node);
    T Visit(BoundVariableDeclaration node);

    // Expressions
    T Visit(BoundNopExpression node);
    T Visit(BoundBinaryExpression node);
    T Visit(BoundBlockExpression node);
    T Visit(BoundLambdaExpression node);
    T Visit(BoundAssignmentExpression node);
    T Visit(BoundLiteralExpression node);
    T Visit(BoundReference node);
    T Visit(BoundNeverExpression node);
    T Visit(BoundUnaryExpression node);
    T Visit(BoundInvocationExpression node);
}

internal static class BoundNodeVisitorExtensions
{
    extension<T>(IBoundNodeVisitor<T> visitor)
    {
        public T Visit(BoundNode node) => node.BoundKind switch
        {
            BoundKind.Unbound => throw CreateUnsupportedNodeKindException(node),
            BoundKind.CompilationUnit => throw CreateUnsupportedNodeKindException(node),
            BoundKind.NopExpression => visitor.Visit((BoundNopExpression)node),
            BoundKind.NeverExpression => visitor.Visit((BoundNeverExpression)node),
            BoundKind.StackInstantiation => throw CreateUnsupportedNodeKindException(node),
            BoundKind.LiteralExpression => visitor.Visit((BoundLiteralExpression)node),
            BoundKind.LambdaExpression => visitor.Visit((BoundLambdaExpression)node),
            BoundKind.AssignmentExpression => visitor.Visit((BoundAssignmentExpression)node),
            BoundKind.PredefinedDeclaration => visitor.Visit((BoundPredefinedDeclaration)node),
            BoundKind.ModuleDeclaration => visitor.Visit((BoundModuleDeclaration)node),
            BoundKind.LabelDeclaration => throw CreateUnsupportedNodeKindException(node),
            BoundKind.StructDeclaration => visitor.Visit((BoundStructDeclaration)node),
            BoundKind.VariableDeclaration => visitor.Visit((BoundVariableDeclaration)node),
            BoundKind.PropertyDeclaration => visitor.Visit((BoundPropertyDeclaration)node),
            BoundKind.MethodDeclaration => throw CreateUnsupportedNodeKindException(node),
            BoundKind.OperatorDeclaration => throw CreateUnsupportedNodeKindException(node),
            BoundKind.ConversionDeclaration => throw CreateUnsupportedNodeKindException(node),
            BoundKind.Reference => visitor.Visit((BoundReference)node),
            BoundKind.BlockExpression => visitor.Visit((BoundBlockExpression)node),
            BoundKind.ArrayInitExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.StructInitExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.PropertyInitExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.InvocationExpression => visitor.Visit((BoundInvocationExpression)node),
            BoundKind.ConversionExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.UnaryExpression => visitor.Visit((BoundUnaryExpression)node),
            BoundKind.BinaryExpression => visitor.Visit((BoundBinaryExpression)node),
            BoundKind.IfExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.WhileExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.BreakExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.ContinueExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.ReturnExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.GotoExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.ConditionalGotoExpression => throw CreateUnsupportedNodeKindException(node),
            _ => throw new ArgumentOutOfRangeException(nameof(node), node.BoundKind, null)
        };
    }

    private static NotSupportedException CreateUnsupportedNodeKindException(BoundNode node) =>
        new($"No concrete visitor overload exists for bound kind '{node.BoundKind}'.");
}
