using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal abstract record class BoundLoopBodyStatement(BoundNodeKind NodeKind, SyntaxNode Syntax, LabelSymbol Break, LabelSymbol Continue)
    : BoundStatement(NodeKind, Syntax);