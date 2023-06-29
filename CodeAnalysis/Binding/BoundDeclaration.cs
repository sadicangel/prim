using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal abstract record class BoundDeclaration(Symbol Symbol, BoundNodeKind DeclarationKind) : BoundStatement(DeclarationKind);
