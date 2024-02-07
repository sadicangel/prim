using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CodeAnalysis.Syntax;
internal static class TokenColours
{
    public const string CommentColour = "darkgreen";
    public const string ControlFlowColour = "purple";
    public const string KeywordColour = "blue3";
    public const string IdentifierColour = "deepskyblue4_2";
    public const string NumberColour = "gold3";
    public const string StringColour = "darkorange3";
    public const string TypeColour = "darkseagreen2";

    public static bool TryGetColour(this TokenKind kind, [MaybeNullWhen(false)] out string colour)
    {
        return (colour = kind switch
        {
            TokenKind.Str => StringColour,
            TokenKind.Identifier => IdentifierColour,
            _ when kind.IsComment() => CommentColour,
            _ when kind.IsControlFlow() => ControlFlowColour,
            _ when kind.IsPredefinedType() => TypeColour,
            _ when kind.IsNumber() => NumberColour,
            _ when kind.IsKeyword() => KeywordColour,
            _ => null,
        }) is not null;
    }
    public static StringBuilder Node(this StringBuilder builder, SyntaxNode node) { node.WriteMarkupTo(builder); return builder; }
    public static StringBuilder Comment(this StringBuilder builder, Token? token) => builder.Token(token, CommentColour);
    public static StringBuilder ControlFlow(this StringBuilder builder, Token? token) => builder.Token(token, ControlFlowColour);
    public static StringBuilder Keyword(this StringBuilder builder, Token? token) => builder.Token(token, KeywordColour);
    public static StringBuilder Identifier(this StringBuilder builder, Token? token) => builder.Token(token, IdentifierColour);
    public static StringBuilder Literal(this StringBuilder builder, Token? token) => token is not null ? token.TokenKind.TryGetColour(out var colour) ? builder.Token(token, colour) : builder.Token(token) : builder;
    public static StringBuilder Number(this StringBuilder builder, Token? token) => builder.Token(token, NumberColour);
    public static StringBuilder String(this StringBuilder builder, Token? token) => builder.Token(token, StringColour);
    public static StringBuilder Type(this StringBuilder builder, Token? token) => builder.Token(token, TypeColour);
    public static StringBuilder Token(this StringBuilder builder, Token? token)
    {
        if (token is not null)
        {
            foreach (var trivia in token.Trivia.Leading)
                builder.Append(trivia.Text);
            builder.Append(token.Text);
            foreach (var trivia in token.Trivia.Trailing)
                builder.Append(trivia.Text);
        }
        return builder;
    }
    public static StringBuilder Token(this StringBuilder builder, Token? token, string style)
    {
        if (token is not null)
        {
            foreach (var trivia in token.Trivia.Leading)
                builder.Append(trivia.Text);
            builder.Append($"[{style}]{token.Text}[/]");
            foreach (var trivia in token.Trivia.Trailing)
                builder.Append(trivia.Text);
        }
        return builder;
    }
}