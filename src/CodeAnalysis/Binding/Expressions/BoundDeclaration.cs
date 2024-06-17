using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundDeclaration(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    PrimType Type)
    : BoundExpression(BoundKind, Syntax, Type);
