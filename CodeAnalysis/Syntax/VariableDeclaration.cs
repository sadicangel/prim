﻿using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Syntax;

public sealed record class VariableDeclaration(Token Modifier, Token Identifier, Token? Colon, Token? Type, Token Equal, Expression Expression, Token Semicolon) : Declaration(DeclarationKind.Variable)
{
    public VariableDeclaration(Token storageToken, Token identifierToken, Token equalsToken, Expression expression, Token semicolonToken)
        : this(storageToken, identifierToken, Colon: null, Type: null, equalsToken, expression, semicolonToken) { }

    [MemberNotNullWhen(true, nameof(Colon), nameof(Type))]
    public bool HasTypeDeclaration { get => Colon is not null && Type is not null; }

    public override T Accept<T>(ISyntaxStatementVisitor<T> visitor) => visitor.Visit(this);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Modifier;
        yield return Identifier;
        if (HasTypeDeclaration)
        {
            yield return Colon;
            yield return Type;
        }
        yield return Equal;
        yield return Expression;
        yield return Semicolon;
    }
}
