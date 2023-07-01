using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal abstract record class BoundLoopBodyStatement(BoundNodeKind NodeKind, LabelSymbol Break, LabelSymbol Continue) : BoundStatement(NodeKind)
{
}