using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundDeclaration(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    PrimType Type)
    : BoundExpression(BoundKind, Syntax, Type);
