using System.Diagnostics;

namespace CodeAnalysis.Syntax;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record class Token(
    SyntaxTree SyntaxTree,
    TokenKind TokenKind,
    Range Range,
    TokenTrivia Trivia,
    object? Value
)
    : SyntaxNode(SyntaxNodeKind.Token, SyntaxTree)
{
    public bool IsMissing { get => SyntaxTree.Source[Range].Length == 0; }

    public override Range Range { get; } = Range;

    public override Range RangeWithWhiteSpace
    {
        get
        {
            var start = Trivia.Leading is [var first, ..] ? first.RangeWithWhiteSpace.Start : Range.Start;
            var end = Trivia.Trailing is [.., var last] ? last.RangeWithWhiteSpace.End : Range.End;
            return start..end;
        }
    }

    public override IEnumerable<SyntaxNode> Children() => Enumerable.Empty<SyntaxNode>();

    private string GetDebuggerDisplay() => $"{TokenKind} {{ \"{Value ?? Text.ToString()}\" }}";
}

public enum TokenKind
{
    // Control
    Invalid,
    EOF,

    // Trivia
    InvalidText,
    LineBreak,
    WhiteSpace,
    SingleLineComment,
    MultiLineComment,

    // Punctuation
    Arrow,
    OpenBrace,
    OpenParenthesis,
    CloseBrace,
    CloseParenthesis,
    Colon,
    Comma,
    Semicolon,

    // Operators
    Ampersand,
    AmpersandAmpersand,
    AmpersandEqual,
    As,
    Bang,
    BangEqual,
    Equal,
    EqualEqual,
    Greater,
    GreaterEqual,
    Hat,
    HatEqual,
    Less,
    LessEqual,
    Minus,
    MinusEqual,
    Percent,
    PercentEqual,
    Pipe,
    PipeEqual,
    PipePipe,
    Plus,
    PlusEqual,
    StarStar,
    StarStarEqual,
    Range,
    Slash,
    SlashEqual,
    Star,
    StarEqual,
    Tilde,

    // Control flow
    If,
    Else,
    While,
    For,
    Result,
    Continue,
    Break,
    Return,

    // Variables
    Mutable,
    I32,
    F32,
    String,
    True,
    False,
    Identifier,
}
