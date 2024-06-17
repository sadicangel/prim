using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundMemberDeclaration(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    PrimType Type)
    : BoundDeclaration(BoundKind, Syntax, Type);
