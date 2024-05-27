namespace CodeAnalysis.Syntax.Types;
public sealed record class TypeSyntaxList(SyntaxTree SyntaxTree, SyntaxList<SyntaxNode> SyntaxNodes)
    : SeparatedSyntaxList<TypeSyntax>(SyntaxKind.TypeList, SyntaxTree, SyntaxNodes);
