using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal abstract record class BoundDeclaration(BoundNodeKind DeclarationKind, Symbol Symbol) : BoundStatement(DeclarationKind);
