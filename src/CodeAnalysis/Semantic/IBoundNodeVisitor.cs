using CodeAnalysis.Semantic.ControlFlow;
using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.References;

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
    T Visit(BoundArrayExpression node);
    T Visit(BoundStructExpression node);
    T Visit(BoundPropertyExpression node);
    T Visit(BoundAssignmentExpression node);
    T Visit(BoundLiteralExpression node);
    T Visit(BoundVariableReference node);
    T Visit(BoundPropertyReference node);
    T Visit(BoundOperatorReference node);
    T Visit(BoundConversionReference node);
    T Visit(BoundElementReference node);
    T Visit(BoundNeverExpression node);
    T Visit(BoundUnaryExpression node);
    T Visit(BoundCallExpression node);
    T Visit(BoundIfElseExpression node);
    T Visit(BoundWhileExpression node);
    T Visit(BoundBreakExpression node);
    T Visit(BoundContinueExpression node);
    T Visit(BoundReturnExpression node);
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
            BoundKind.VariableReference => visitor.Visit((BoundVariableReference)node),
            BoundKind.PropertyReference => visitor.Visit((BoundPropertyReference)node),
            BoundKind.OperatorReference => visitor.Visit((BoundOperatorReference)node),
            BoundKind.ConversionReference => visitor.Visit((BoundConversionReference)node),
            BoundKind.ElementReference => visitor.Visit((BoundElementReference)node),
            BoundKind.BlockExpression => visitor.Visit((BoundBlockExpression)node),
            BoundKind.ArrayExpression => visitor.Visit((BoundArrayExpression)node),
            BoundKind.StructExpression => visitor.Visit((BoundStructExpression)node),
            BoundKind.PropertyExpression => visitor.Visit((BoundPropertyExpression)node),
            BoundKind.CallExpression => visitor.Visit((BoundCallExpression)node),
            BoundKind.ConversionExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.UnaryExpression => visitor.Visit((BoundUnaryExpression)node),
            BoundKind.BinaryExpression => visitor.Visit((BoundBinaryExpression)node),
            BoundKind.IfElseExpression => visitor.Visit((BoundIfElseExpression)node),
            BoundKind.WhileExpression => visitor.Visit((BoundWhileExpression)node),
            BoundKind.BreakExpression => visitor.Visit((BoundBreakExpression)node),
            BoundKind.ContinueExpression => visitor.Visit((BoundContinueExpression)node),
            BoundKind.ReturnExpression => visitor.Visit((BoundReturnExpression)node),
            BoundKind.GotoExpression => throw CreateUnsupportedNodeKindException(node),
            BoundKind.ConditionalGotoExpression => throw CreateUnsupportedNodeKindException(node),
            _ => throw new ArgumentOutOfRangeException(nameof(node), node.BoundKind, null)
        };
    }

    private static NotSupportedException CreateUnsupportedNodeKindException(BoundNode node) =>
        new($"No concrete visitor overload exists for bound kind '{node.BoundKind}'.");
}
