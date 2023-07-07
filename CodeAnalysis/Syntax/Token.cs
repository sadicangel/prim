﻿using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class Token(SyntaxTree SyntaxTree, TokenKind TokenKind, int Position, string Text, object? Value = null)
    : SyntaxNode(SyntaxNodeKind.Token, SyntaxTree)
{
    public bool IsMissing { get => String.IsNullOrEmpty(Text); }

    public override TextSpan Span { get => new(Position, Text.Length); }

    public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();
}