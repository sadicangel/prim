using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Statements;

internal abstract record class BoundDeclaration(BoundNodeKind DeclarationKind, Symbol Symbol) : BoundStatement(DeclarationKind);
