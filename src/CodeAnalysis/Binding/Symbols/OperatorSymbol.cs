using System.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class OperatorSymbol(SyntaxNode Syntax, Operator Operator, bool IsReadOnly = true)
    : Symbol(GetBoundKind(Operator.OperatorKind), Syntax, Operator.Name, IsReadOnly, IsStatic: true)
{
    public override FunctionType Type { get; } = Operator.Type;

    public PrimType ReturnType { get => Type.ReturnType; }

    private static BoundKind GetBoundKind(SyntaxKind operatorKind) => operatorKind switch
    {
        SyntaxKind.IndexOperator => BoundKind.IndexOperator,
        SyntaxKind.InvocationOperator => BoundKind.InvocationOperator,
        SyntaxKind.MemberAccessOperator => BoundKind.MemberAccessOperator,
        SyntaxKind.ConversionOperator => BoundKind.ConversionOperator,
        SyntaxKind.UnaryPlusOperator => BoundKind.UnaryPlusOperator,
        SyntaxKind.UnaryMinusOperator => BoundKind.UnaryMinusOperator,
        SyntaxKind.OnesComplementOperator => BoundKind.OnesComplementOperator,
        SyntaxKind.NotOperator => BoundKind.NotOperator,
        SyntaxKind.AddOperator => BoundKind.AddOperator,
        SyntaxKind.SubtractOperator => BoundKind.SubtractOperator,
        SyntaxKind.MultiplyOperator => BoundKind.MultiplyOperator,
        SyntaxKind.DivideOperator => BoundKind.DivideOperator,
        SyntaxKind.ModuloOperator => BoundKind.ModuloOperator,
        SyntaxKind.PowerOperator => BoundKind.PowerOperator,
        SyntaxKind.LeftShiftOperator => BoundKind.LeftShiftOperator,
        SyntaxKind.RightShiftOperator => BoundKind.RightShiftOperator,
        SyntaxKind.LogicalOrOperator => BoundKind.LogicalOrOperator,
        SyntaxKind.LogicalAndOperator => BoundKind.LogicalAndOperator,
        SyntaxKind.BitwiseOrOperator => BoundKind.BitwiseOrOperator,
        SyntaxKind.BitwiseAndOperator => BoundKind.BitwiseAndOperator,
        SyntaxKind.ExclusiveOrOperator => BoundKind.ExclusiveOrOperator,
        SyntaxKind.EqualsOperator => BoundKind.EqualsOperator,
        SyntaxKind.NotEqualsOperator => BoundKind.NotEqualsOperator,
        SyntaxKind.LessThanOperator => BoundKind.LessThanOperator,
        SyntaxKind.LessThanOrEqualOperator => BoundKind.LessThanOrEqualOperator,
        SyntaxKind.GreaterThanOperator => BoundKind.GreaterThanOperator,
        SyntaxKind.GreaterThanOrEqualOperator => BoundKind.GreaterThanOrEqualOperator,
        SyntaxKind.CoalesceOperator => BoundKind.CoalesceOperator,
        _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{operatorKind}'")
    };
}
