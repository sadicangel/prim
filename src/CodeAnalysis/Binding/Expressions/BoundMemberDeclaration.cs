using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundMemberDeclaration(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode,
    PrimType Type)
    : BoundDeclaration(BoundKind, SyntaxNode, Type);
