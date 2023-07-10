using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Statements;

internal abstract record class BoundLoopBodyStatement(BoundNodeKind NodeKind, LabelSymbol Break, LabelSymbol Continue) : BoundStatement(NodeKind)
{
}