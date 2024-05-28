﻿namespace CodeAnalysis.Syntax;
public enum SyntaxKind
{
    InvalidSyntax,

    // SyntaxTokens

    EofToken,
    IdentifierToken,

    I32LiteralToken,
    U32LiteralToken,
    I64LiteralToken,
    U64LiteralToken,
    F32LiteralToken,
    F64LiteralToken,
    StrLiteralToken,

    AmpersandToken,
    AmpersandAmpersandToken,
    AmpersandEqualsToken,
    BangToken,
    BangEqualsToken,
    BraceOpenToken,
    BraceCloseToken,
    BracketOpenToken,
    BracketCloseToken,
    ColonToken,
    CommaToken,
    DotToken,
    DotDotToken,
    EqualsToken,
    EqualsEqualsToken,
    MinusGreaterThanToken,
    GreaterThanToken,
    GreaterThanEqualsToken,
    GreaterThanGreaterThanToken,
    GreaterThanGreaterThanEqualsToken,
    HatToken,
    HatEqualsToken,
    HookToken,
    HookHookToken,
    HookHookEqualsToken,
    LessThanToken,
    LessThanEqualsToken,
    LessThanLessThanToken,
    LessThanLessThanEqualsToken,
    MinusToken,
    MinusEqualsToken,
    MinusMinusToken,
    ParenthesisOpenToken,
    ParenthesisCloseToken,
    PercentToken,
    PercentEqualsToken,
    PipeToken,
    PipeEqualsToken,
    PipePipeToken,
    PlusToken,
    PlusEqualsToken,
    PlusPlusToken,
    SemicolonToken,
    SlashToken,
    SlashEqualsToken,
    StarToken,
    StarEqualsToken,
    StarStarToken,
    StarStarEqualsToken,
    TildeToken,

    IfKeyword,
    ElseKeyword,
    WhileKeyword,
    ForKeyword,
    ContinueKeyword,
    BreakKeyword,
    ReturnKeyword,

    AnyKeyword,
    UnknownKeyword,
    NeverKeyword,
    UnitKeyword,
    TypeKeyword,
    StrKeyword,
    BoolKeyword,
    I8Keyword,
    I16Keyword,
    I32Keyword,
    I64Keyword,
    I128Keyword,
    ISizeKeyword,
    U8Keyword,
    U16Keyword,
    U32Keyword,
    U64Keyword,
    U128Keyword,
    USizeKeyword,
    F16Keyword,
    F32Keyword,
    F64Keyword,
    F80Keyword,
    F128Keyword,

    StructKeyword,

    TrueKeyword,
    FalseKeyword,
    NullKeyword,

    LineBreakTrivia,
    WhiteSpaceTrivia,
    SingleLineCommentTrivia,
    MultiLineCommentTrivia,
    InvalidTextTrivia,

    // Other nodes

    CompilationUnit,

    // Expressions

    IdentifierNameExpression,

    I32LiteralExpression,
    U32LiteralExpression,
    I64LiteralExpression,
    U64LiteralExpression,
    F32LiteralExpression,
    F64LiteralExpression,
    StrLiteralExpression,
    TrueLiteralExpression,
    FalseLiteralExpression,
    NullLiteralExpression,

    PredefinedType,
    NamedType,
    OptionType,
    ArrayType,
    FunctionType,
    UnionType,
    Parameter,
    ParameterList,
    TypeList,

    GroupExpression,
    SimpleAssignmentExpression,
    AddAssignmentExpression,
    SubtractAssignmentExpression,
    MultiplyAssignmentExpression,
    DivideAssignmentExpression,
    ModuloAssignmentExpression,
    PowerAssignmentExpression,
    AndAssignmentExpression,
    ExclusiveOrAssignmentExpression,
    OrAssignmentExpression,
    LeftShiftAssignmentExpression,
    RightShiftAssignmentExpression,
    CoalesceAssignmentExpression,

    VariableDeclaration,
    FunctionDeclaration,
    StructDeclaration,
    MemberDeclaration,
    LocalDeclaration,

    EmptyExpression,
    StatementExpression,
    BlockExpression,

    UnaryPlusExpression,
    UnaryMinusExpression,
    PrefixIncrementExpression,
    PrefixDecrementExpression,
    OnesComplementExpression,
    NotExpression,

    AddExpression,
    SubtractExpression,
    MultiplyExpression,
    DivideExpression,
    ModuloExpression,
    PowerExpression,
    LeftShiftExpression,
    RightShiftExpression,
    LogicalOrExpression,
    LogicalAndExpression,
    BitwiseOrExpression,
    BitwiseAndExpression,
    ExclusiveOrExpression,
    EqualsExpression,
    NotEqualsExpression,
    LessThanExpression,
    LessThanOrEqualExpression,
    GreaterThanExpression,
    GreaterThanOrEqualExpression,
    CoalesceExpression,
}
