namespace CodeAnalysis.Syntax.Types;
public sealed record class TypeSyntaxList(SyntaxTree SyntaxTree, IReadOnlyList<SyntaxNode> SyntaxNodes)
    : SeparatedSyntaxList<TypeSyntax>(SyntaxKind.TypeList, SyntaxTree, SyntaxNodes);
