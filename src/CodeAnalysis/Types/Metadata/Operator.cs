using System.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Types.Metadata;
public sealed record class Operator(SyntaxKind OperatorKind, FunctionType Type, PrimType ContainingType)
    : Member(GetOperatorName(OperatorKind, Type), ContainingType)
{
    public override FunctionType Type { get; } = Type;

    public PrimType ReturnType { get => Type.ReturnType; }

    public override string ToString() => $"{Name}: {Type.Name}";

    private static string GetOperatorName(SyntaxKind operatorKind, FunctionType type)
    {
        var prefix = operatorKind switch
        {
            SyntaxKind.IndexOperator => "[]",
            SyntaxKind.InvocationOperator => "()",
            SyntaxKind.MemberAccessOperator => ".",
            SyntaxKind.ConversionOperator => "as",

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

        var name = $"{prefix}({string.Join(',', type.Parameters.Select(p => p.Type.Name))})->{type.ReturnType}";

        return name;
    }
}
