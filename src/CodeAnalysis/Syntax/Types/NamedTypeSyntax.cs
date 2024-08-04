﻿using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Syntax.Types;
public sealed record class NamedTypeSyntax(SyntaxTree SyntaxTree, SimpleNameSyntax Name)
    : TypeSyntax(SyntaxKind.NamedType, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
    }
}
