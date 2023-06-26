using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal abstract record class BoundDeclaration(Symbol Symbol) : BoundStatement(BoundNodeKind.Declaration);
