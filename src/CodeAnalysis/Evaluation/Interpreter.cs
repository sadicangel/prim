using CodeAnalysis.Evaluation.Values;
using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Evaluation;

internal sealed class Interpreter : IBoundNodeVisitor<PrimValue>
{
    private readonly Dictionary<Symbol, PrimValue> _evaluations = [];

    public PrimValue Interpret(BoundNode node) => this.Visit(node);

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundModuleDeclaration node)
    {
        throw new NotImplementedException();
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundPredefinedDeclaration node)
    {
        throw new NotImplementedException();
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundPropertyDeclaration node)
    {
        throw new NotImplementedException();
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundStructDeclaration node)
    {
        throw new NotImplementedException();
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundVariableDeclaration node)
    {
        throw new NotImplementedException();
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundBinaryExpression node)
    {
        var left = Interpret(node.Left);
        var right = Interpret(node.Right);
        var @operator = Interpret(node.Operator);

        return ((LambdaValue)@operator).Invoke(left, right);
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundBlockExpression node)
    {
        PrimValue? result = null;
        foreach (var child in node.Expressions)
        {
            result = Interpret(child);
        }

        return result ?? throw new NotImplementedException();
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundLambdaExpression node)
    {
        throw new NotImplementedException();
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundLiteralExpression node)
    {
        throw new NotImplementedException();
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundReference node)
    {
        throw new NotImplementedException();
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundNeverExpression node)
    {
        throw new NotImplementedException();
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundUnaryExpression node)
    {
        throw new NotImplementedException();
    }
}
