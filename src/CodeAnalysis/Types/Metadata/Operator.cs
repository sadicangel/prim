using CodeAnalysis.Syntax;

namespace CodeAnalysis.Types.Metadata;
public sealed record class Operator(SyntaxKind OperatorKind, FunctionType Type, PrimType ContainingType)
    : Member(SyntaxFacts.GetOperatorName(OperatorKind), ContainingType)
{
    public override FunctionType Type { get; } = Type;
    public override string ToString() => $"{Name}: {Type.Name}";
}
