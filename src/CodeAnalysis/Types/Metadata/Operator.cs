using CodeAnalysis.Syntax;

namespace CodeAnalysis.Types.Metadata;
public sealed record class Operator(SyntaxKind OperatorKind, FunctionType Type, PrimType ContainingType)
    : Member(SyntaxFacts.GetMethodName(OperatorKind, Type), ContainingType, IsReadOnly: true, IsStatic: true)
{
    public override FunctionType Type { get; } = Type;

    public PrimType ReturnType { get => Type.ReturnType; }

    public override string ToString() => $"{Name}: {Type.Name}";
}
