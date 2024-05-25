namespace CodeAnalysis.Syntax.Types;
public sealed record class TypeSyntaxList(SyntaxTree SyntaxTree, ReadOnlyList<SyntaxNode> SyntaxNodes)
    : SeparatedSyntaxList<TypeSyntax>(SyntaxKind.TypeList, SyntaxTree, SyntaxNodes);
