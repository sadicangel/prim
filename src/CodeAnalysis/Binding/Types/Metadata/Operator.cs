using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Types.Metadata;
internal sealed record class Operator(SyntaxKind OperatorKind, FunctionType Type)
    : Member(SyntaxFacts.GetOperatorName(OperatorKind))
{
    public override FunctionType Type { get; } = Type;
    public override string ToString() => $"{Name}: {Type.Name}";
}
