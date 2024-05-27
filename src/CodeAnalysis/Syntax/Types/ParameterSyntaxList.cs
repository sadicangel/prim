namespace CodeAnalysis.Syntax.Types;

public sealed record class ParameterSyntaxList(SyntaxTree SyntaxTree, SyntaxList<SyntaxNode> SyntaxNodes)
    : SeparatedSyntaxList<ParameterSyntax>(SyntaxKind.ParameterList, SyntaxTree, SyntaxNodes);
