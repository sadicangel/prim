using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Statements;

internal abstract record class BoundDeclaration(BoundNodeKind DeclarationKind, SyntaxNode Syntax, Symbol Symbol)
    : BoundStatement(DeclarationKind, Syntax);
