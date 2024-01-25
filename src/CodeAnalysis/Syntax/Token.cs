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
    // Invalid
    Invalid,

    // Punctuation
    Brace_Open,
    Brace_Close,
    Parenthesis_Open,
    Parenthesis_Close,
    Colon,
    Semicolon,
    Comma,

    // Operators
    Ampersand,
    AmpersandAmpersand,
    AmpersandEqual,
    Bang,
    BangEqual,
    // Call,
    Cast,
    Dot,
    DotDot,
    Equal,
    EqualEqual,
    Greater,
    GreaterEqual,
    GreaterGreater,
    GreaterGreaterEqual,
    Hat,
    HatEqual,
    Hook,
    HookHook,
    HookHookEqual,
    Lambda,
    Less,
    LessEqual,
    LessLess,
    LessLessEqual,
    Minus,
    MinusEqual,
    MinusMinus,
    Percent,
    PercentEqual,
    Pipe,
    PipeEqual,
    PipePipe,
    Plus,
    PlusEqual,
    PlusPlus,
    // SizeOf,
    Slash,
    SlashEqual,
    Star,
    StarEqual,
    StarStar,
    StarStarEqual,
    // Subscript,
    Tilde,
    // TypeOf,

    // Control flow
    If,
    Else,
    While,
    For,
    Continue,
    Break,
    Return,

    // Literals
    I32,
    //I64,
    F32,
    //F64,
    Str,
    True,
    False,
    Null,

    // Other Keywords
    Mutable,
    SizeOf,
    TypeOf,

    // Primitive Types
    Type_Any,
    Type_Never,
    Type_Unit,
    Type_Type,
    //Type_Option,
    //Type_Union,
    //Type_Array,
    Type_Str,
    //Type_Func,
    Type_Bool,
    Type_I8,
    Type_I16,
    Type_I32,
    Type_I64,
    Type_I128,
    Type_ISize,
    Type_U8,
    Type_U16,
    Type_U32,
    Type_U64,
    Type_U128,
    Type_USize,
    Type_F16,
    Type_F32,
    Type_F64,
    Type_F80,
    Type_F128,

    // Identifier
    Identifier,

    // Trivia
    LineBreak,
    WhiteSpace,
    Comment_SingleLine,
    Comment_MultiLine,
    InvalidText,

    // EOF
    EOF,
}
