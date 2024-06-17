using System.Diagnostics;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class OperatorSymbol(
    SyntaxNode Syntax,
    Operator Operator)
    : MemberSymbol(GetBoundKind(Operator.OperatorKind), Syntax, $"{GetOperatorName(Operator.OperatorKind)}<{Operator.Type.Name}>")
{
    public override FunctionType Type { get; } = Operator.Type;

    private static string GetOperatorName(SyntaxKind operatorKind) => operatorKind switch
    {
        SyntaxKind.UnaryPlusOperator => "+",
        SyntaxKind.UnaryMinusOperator => "-",
        SyntaxKind.OnesComplementOperator => "~",
        SyntaxKind.NotOperator => "!",
        SyntaxKind.AddOperator => "+",
        SyntaxKind.SubtractOperator => "-",
        SyntaxKind.MultiplyOperator => "*",
        SyntaxKind.DivideOperator => "/",
        SyntaxKind.ModuloOperator => "%",
        SyntaxKind.PowerOperator => "**",
        SyntaxKind.LeftShiftOperator => "<<",
        SyntaxKind.RightShiftOperator => ">>",
        SyntaxKind.LogicalOrOperator => "||",
        SyntaxKind.LogicalAndOperator => "&&",
        SyntaxKind.BitwiseOrOperator => "|",
        SyntaxKind.BitwiseAndOperator => "&",
        SyntaxKind.ExclusiveOrOperator => "^",
        SyntaxKind.EqualsOperator => "==",
        SyntaxKind.NotEqualsOperator => "!=",
        SyntaxKind.LessThanOperator => "<",
        SyntaxKind.LessThanOrEqualOperator => "<=",
        SyntaxKind.GreaterThanOperator => ">",
        SyntaxKind.GreaterThanOrEqualOperator => ">=",
        SyntaxKind.CoalesceOperator => "??",
        _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)} '{operatorKind}'"),
    };

    private static BoundKind GetBoundKind(SyntaxKind operatorKind) => operatorKind switch
    {
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
