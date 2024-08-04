﻿using CodeAnalysis.Syntax.Expressions.Names;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class VariableDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    TypeSyntax? Type,
    InitValueExpressionSyntax? InitValue,
    SyntaxToken? SemicolonToken)
    : DeclarationSyntax(SyntaxKind.VariableDeclaration, SyntaxTree)
{
    public bool IsReadOnly { get => InitValue?.IsReadOnly ?? false; }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        if (Type is not null)
            yield return Type;
        if (InitValue is not null)
            yield return InitValue;
        if (SemicolonToken is not null)
            yield return SemicolonToken;
    }
}
