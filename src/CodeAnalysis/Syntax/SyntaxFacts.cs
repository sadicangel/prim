using System.Diagnostics;

namespace CodeAnalysis.Syntax;

public static class SyntaxFacts
{
    public static string? GetText(SyntaxKind syntaxKind)
    {
        return syntaxKind switch
        {
            SyntaxKind.InvalidSyntax => null,
            SyntaxKind.EofToken => null,
            SyntaxKind.IdentifierToken => null,

            SyntaxKind.AmpersandToken => "&",
            SyntaxKind.AmpersandAmpersandToken => "&&",
            SyntaxKind.AmpersandEqualsToken => "&=",
            SyntaxKind.ExclamationToken => "!",
            SyntaxKind.ExclamationEqualsToken => "!=",
            SyntaxKind.BraceOpenToken => "{",
            SyntaxKind.BraceCloseToken => "}",
            SyntaxKind.BracketOpenToken => "[",
            SyntaxKind.BracketCloseToken => "]",
            SyntaxKind.BracketOpenBracketCloseToken => "[]",
            SyntaxKind.ColonToken => ":",
            SyntaxKind.ColonColonToken => "::",
            SyntaxKind.CommaToken => ",",
            SyntaxKind.DotToken => ".",
            SyntaxKind.DotDotToken => "..",
            SyntaxKind.EqualsToken => "=",
            SyntaxKind.EqualsEqualsToken => "==",
            SyntaxKind.GreaterThanToken => ">",
            SyntaxKind.GreaterThanEqualsToken => ">=",
            SyntaxKind.GreaterThanGreaterThanToken => ">>",
            SyntaxKind.GreaterThanGreaterThanEqualsToken => ">>=",
            SyntaxKind.CaretToken => "^",
            SyntaxKind.CaretEqualsToken => "^=",
            SyntaxKind.HookToken => "?",
            SyntaxKind.HookHookToken => "??",
            SyntaxKind.HookHookEqualsToken => "??=",
            SyntaxKind.LessThanToken => "<",
            SyntaxKind.LessThanEqualsToken => "<=",
            SyntaxKind.LessThanLessThanToken => "<<",
            SyntaxKind.LessThanLessThanEqualsToken => "<<=",
            SyntaxKind.MinusToken => "-",
            SyntaxKind.MinusEqualsToken => "-=",
            SyntaxKind.ParenthesisOpenToken => "(",
            SyntaxKind.ParenthesisCloseToken => ")",
            SyntaxKind.ParenthesisOpenParenthesisCloseToken => "()",
            SyntaxKind.PercentToken => "%",
            SyntaxKind.PercentEqualsToken => "%=",
            SyntaxKind.BarToken => "|",
            SyntaxKind.PipeEqualsToken => "|=",
            SyntaxKind.BarBarToken => "||",
            SyntaxKind.PlusToken => "+",
            SyntaxKind.PlusEqualsToken => "+=",
            SyntaxKind.SemicolonToken => ";",
            SyntaxKind.SlashToken => "/",
            SyntaxKind.SlashEqualsToken => "/=",
            SyntaxKind.AsteriskToken => "*",
            SyntaxKind.StarEqualsToken => "*=",
            SyntaxKind.AsteriskAsteriskToken => "**",
            SyntaxKind.StarStarEqualsToken => "**=",
            SyntaxKind.TildeToken => "~",

            SyntaxKind.EqualsGreaterThanToken => "=>",
            SyntaxKind.MinusGreaterThanToken => "->",

            SyntaxKind.I8LiteralToken => null,
            SyntaxKind.U8LiteralToken => null,
            SyntaxKind.I16LiteralToken => null,
            SyntaxKind.U16LiteralToken => null,
            SyntaxKind.I32LiteralToken => null,
            SyntaxKind.U32LiteralToken => null,
            SyntaxKind.I64LiteralToken => null,
            SyntaxKind.U64LiteralToken => null,
            SyntaxKind.F16LiteralToken => null,
            SyntaxKind.F32LiteralToken => null,
            SyntaxKind.F64LiteralToken => null,
            SyntaxKind.StrLiteralToken => null,

            SyntaxKind.AsKeyword => "as",
            SyntaxKind.IfKeyword => "if",
            SyntaxKind.ImplicitKeyword => "implicit",
            SyntaxKind.ElseKeyword => "else",
            SyntaxKind.ExplicitKeyword => "explicit",
            SyntaxKind.WhileKeyword => "while",
            SyntaxKind.ForKeyword => "for",
            SyntaxKind.ContinueKeyword => "continue",
            SyntaxKind.BreakKeyword => "break",
            SyntaxKind.ReturnKeyword => "return",

            SyntaxKind.AnyKeyword => PredefinedTypeNames.Any,
            SyntaxKind.UnknownKeyword => PredefinedTypeNames.Unknown,
            SyntaxKind.NeverKeyword => PredefinedTypeNames.Never,
            SyntaxKind.UnitKeyword => PredefinedTypeNames.Unit,
            SyntaxKind.TypeKeyword => PredefinedTypeNames.Type,
            SyntaxKind.StrKeyword => PredefinedTypeNames.Str,
            SyntaxKind.BoolKeyword => PredefinedTypeNames.Bool,
            SyntaxKind.I8Keyword => PredefinedTypeNames.I8,
            SyntaxKind.I16Keyword => PredefinedTypeNames.I16,
            SyntaxKind.I32Keyword => PredefinedTypeNames.I32,
            SyntaxKind.I64Keyword => PredefinedTypeNames.I64,
            SyntaxKind.IszKeyword => PredefinedTypeNames.Isz,
            SyntaxKind.U8Keyword => PredefinedTypeNames.U8,
            SyntaxKind.U16Keyword => PredefinedTypeNames.U16,
            SyntaxKind.U32Keyword => PredefinedTypeNames.U32,
            SyntaxKind.U64Keyword => PredefinedTypeNames.U64,
            SyntaxKind.UszKeyword => PredefinedTypeNames.Usz,
            SyntaxKind.F16Keyword => PredefinedTypeNames.F16,
            SyntaxKind.F32Keyword => PredefinedTypeNames.F32,
            SyntaxKind.F64Keyword => PredefinedTypeNames.F64,

            SyntaxKind.ModuleKeyword => "module",
            SyntaxKind.StructKeyword => "struct",
            SyntaxKind.LetKeyword => "let",
            SyntaxKind.VarKeyword => "var",

            SyntaxKind.TrueKeyword => "true",
            SyntaxKind.FalseKeyword => "false",
            SyntaxKind.NullKeyword => "null",

            SyntaxKind.ThisKeyword => "this",

            SyntaxKind.LineBreakTrivia => null,
            SyntaxKind.WhiteSpaceTrivia => null,
            SyntaxKind.SingleLineCommentTrivia => null,
            SyntaxKind.MultiLineCommentTrivia => null,
            SyntaxKind.InvalidTextTrivia => null,

            SyntaxKind.CompilationUnit => null,

            SyntaxKind.PredefinedType => null,
            SyntaxKind.NamedType => null,
            SyntaxKind.MaybeType => null,
            SyntaxKind.PointerType => null,
            SyntaxKind.ArrayType => null,
            SyntaxKind.LambdaType => null,
            SyntaxKind.UnionType => null,

            SyntaxKind.Parameter => null,
            SyntaxKind.Argument => null,

            SyntaxKind.SimpleName => null,
            SyntaxKind.QualifiedName => null,

            SyntaxKind.I8LiteralExpression => null,
            SyntaxKind.U8LiteralExpression => null,
            SyntaxKind.I16LiteralExpression => null,
            SyntaxKind.U16LiteralExpression => null,
            SyntaxKind.I32LiteralExpression => null,
            SyntaxKind.U32LiteralExpression => null,
            SyntaxKind.I64LiteralExpression => null,
            SyntaxKind.U64LiteralExpression => null,
            SyntaxKind.F16LiteralExpression => null,
            SyntaxKind.F32LiteralExpression => null,
            SyntaxKind.F64LiteralExpression => null,
            SyntaxKind.StrLiteralExpression => null,
            SyntaxKind.TrueLiteralExpression => null,
            SyntaxKind.FalseLiteralExpression => null,
            SyntaxKind.NullLiteralExpression => null,

            SyntaxKind.GroupExpression => null,

            SyntaxKind.ModuleDeclaration => null,
            SyntaxKind.StructDeclaration => null,
            SyntaxKind.VariableDeclaration => null,
            SyntaxKind.PropertyDeclaration => null,

            SyntaxKind.ArrayExpression => null,
            SyntaxKind.StructExpression => null,
            SyntaxKind.PropertyExpression => null,

            SyntaxKind.ElementAccessExpression => null,
            SyntaxKind.InvocationExpression => null,
            SyntaxKind.PropertyAccessExpression => null,
            SyntaxKind.ConversionExpression => null,

            SyntaxKind.UnaryPlusExpression => null,
            SyntaxKind.UnaryMinusExpression => null,
            SyntaxKind.OnesComplementExpression => null,
            SyntaxKind.NotExpression => null,

            SyntaxKind.AddExpression => null,
            SyntaxKind.SubtractExpression => null,
            SyntaxKind.MultiplyExpression => null,
            SyntaxKind.DivideExpression => null,
            SyntaxKind.ModuloExpression => null,
            SyntaxKind.PowerExpression => null,
            SyntaxKind.LeftShiftExpression => null,
            SyntaxKind.RightShiftExpression => null,
            SyntaxKind.LogicalOrExpression => null,
            SyntaxKind.LogicalAndExpression => null,
            SyntaxKind.BitwiseOrExpression => null,
            SyntaxKind.BitwiseAndExpression => null,
            SyntaxKind.ExclusiveOrExpression => null,
            SyntaxKind.EqualsExpression => null,
            SyntaxKind.NotEqualsExpression => null,
            SyntaxKind.LessThanExpression => null,
            SyntaxKind.LessThanOrEqualExpression => null,
            SyntaxKind.GreaterThanExpression => null,
            SyntaxKind.GreaterThanOrEqualExpression => null,
            SyntaxKind.CoalesceExpression => null,

            SyntaxKind.AssignmentExpression => null,
            SyntaxKind.TypeClause => null,
            SyntaxKind.InitClause => null,

            SyntaxKind.EmptyStatement => null,
            SyntaxKind.ExpressionStatement => null,
            SyntaxKind.BlockStatement => null,

            SyntaxKind.IfElseStatement => null,
            SyntaxKind.ElseClause => null,
            SyntaxKind.WhileStatement => null,

            SyntaxKind.ContinueStatement => null,
            SyntaxKind.BreakStatement => null,
            SyntaxKind.ReturnStatement => null,

            _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)}: '{syntaxKind}'")
        };
    }

    public static SyntaxKind GetKeywordKind(ReadOnlySpan<char> syntaxText)
    {
        return syntaxText switch
        {
            "as" => SyntaxKind.AsKeyword,
            "if" => SyntaxKind.IfKeyword,
            "implicit" => SyntaxKind.ImplicitKeyword,
            "else" => SyntaxKind.ElseKeyword,
            "explicit" => SyntaxKind.ExplicitKeyword,
            "while" => SyntaxKind.WhileKeyword,
            "for" => SyntaxKind.ForKeyword,
            "continue" => SyntaxKind.ContinueKeyword,
            "break" => SyntaxKind.BreakKeyword,
            "return" => SyntaxKind.ReturnKeyword,

            PredefinedTypeNames.Any => SyntaxKind.AnyKeyword,
            PredefinedTypeNames.Unknown => SyntaxKind.UnknownKeyword,
            PredefinedTypeNames.Never => SyntaxKind.NeverKeyword,
            PredefinedTypeNames.Unit => SyntaxKind.UnitKeyword,
            PredefinedTypeNames.Type => SyntaxKind.TypeKeyword,
            PredefinedTypeNames.Str => SyntaxKind.StrKeyword,
            PredefinedTypeNames.Bool => SyntaxKind.BoolKeyword,
            PredefinedTypeNames.I8 => SyntaxKind.I8Keyword,
            PredefinedTypeNames.I16 => SyntaxKind.I16Keyword,
            PredefinedTypeNames.I32 => SyntaxKind.I32Keyword,
            PredefinedTypeNames.I64 => SyntaxKind.I64Keyword,
            PredefinedTypeNames.Isz => SyntaxKind.IszKeyword,
            PredefinedTypeNames.U8 => SyntaxKind.U8Keyword,
            PredefinedTypeNames.U16 => SyntaxKind.U16Keyword,
            PredefinedTypeNames.U32 => SyntaxKind.U32Keyword,
            PredefinedTypeNames.U64 => SyntaxKind.U64Keyword,
            PredefinedTypeNames.Usz => SyntaxKind.UszKeyword,
            PredefinedTypeNames.F16 => SyntaxKind.F16Keyword,
            PredefinedTypeNames.F32 => SyntaxKind.F32Keyword,
            PredefinedTypeNames.F64 => SyntaxKind.F64Keyword,

            "module" => SyntaxKind.ModuleKeyword,
            "struct" => SyntaxKind.StructKeyword,
            "let" => SyntaxKind.LetKeyword,
            "var" => SyntaxKind.VarKeyword,

            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            "null" => SyntaxKind.NullKeyword,

            "this" => SyntaxKind.ThisKeyword,

            _ => SyntaxKind.IdentifierToken,
        };
    }

    public static bool IsOperator(SyntaxKind syntaxKind) => IsUnaryOperator(syntaxKind) || IsBinaryOperator(syntaxKind);
    public static bool IsUnaryOperator(SyntaxKind syntaxKind) => GetUnaryOperatorPrecedence(syntaxKind) > 0;
    public static bool IsBinaryOperator(SyntaxKind syntaxKind) => GetBinaryOperatorPrecedence(syntaxKind) > 0;

    public static bool IsKeyword(SyntaxKind syntaxKind) =>
        syntaxKind is >= SyntaxKind.IfKeyword and <= SyntaxKind.NullKeyword;

    public static bool IsPredefinedType(SyntaxKind syntaxKind) =>
        syntaxKind is >= SyntaxKind.AnyKeyword and <= SyntaxKind.F64Keyword;

    public static bool IsNumberLiteralToken(SyntaxKind syntaxKind) =>
        syntaxKind is >= SyntaxKind.I32LiteralToken and <= SyntaxKind.F64LiteralToken;

    public static int? GetUnaryOperatorPrecedence(SyntaxKind syntaxKind) => syntaxKind switch
    {
        SyntaxKind.ExclamationToken => 8,
        SyntaxKind.MinusToken => 8,
        SyntaxKind.PlusToken => 8,
        SyntaxKind.TildeToken => 8,
        _ => null,
    };

    public static SyntaxKind GetUnaryOperatorExpression(SyntaxKind operatorKind) => operatorKind switch
    {
        SyntaxKind.ExclamationToken => SyntaxKind.NotExpression,
        SyntaxKind.MinusToken => SyntaxKind.UnaryMinusExpression,
        SyntaxKind.PlusToken => SyntaxKind.UnaryPlusExpression,
        SyntaxKind.TildeToken => SyntaxKind.OnesComplementExpression,
        _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)}: '{operatorKind}'")
    };

    public static int? GetBinaryOperatorPrecedence(SyntaxKind syntaxKind) => syntaxKind switch
    {
        SyntaxKind.AsteriskAsteriskToken => 7,
        SyntaxKind.PercentToken => 6,
        SyntaxKind.AsteriskToken => 6,
        SyntaxKind.SlashToken => 6,
        SyntaxKind.PlusToken => 5,
        SyntaxKind.MinusToken => 5,
        SyntaxKind.LessThanLessThanToken => 4,
        SyntaxKind.GreaterThanGreaterThanToken => 4,
        SyntaxKind.EqualsEqualsToken => 3,
        SyntaxKind.ExclamationEqualsToken => 3,
        SyntaxKind.LessThanToken => 3,
        SyntaxKind.LessThanEqualsToken => 3,
        SyntaxKind.GreaterThanToken => 3,
        SyntaxKind.GreaterThanEqualsToken => 3,
        SyntaxKind.AmpersandToken => 2,
        SyntaxKind.AmpersandAmpersandToken => 2,
        SyntaxKind.BarToken => 1,
        SyntaxKind.BarBarToken => 1,
        SyntaxKind.CaretToken => 1,
        SyntaxKind.HookHookToken => 1,
        _ => null,
    };

    public static SyntaxKind GetBinaryOperatorExpression(SyntaxKind operatorKind) => operatorKind switch
    {
        SyntaxKind.AsteriskAsteriskToken => SyntaxKind.PowerExpression,
        SyntaxKind.PercentToken => SyntaxKind.ModuloExpression,
        SyntaxKind.AsteriskToken => SyntaxKind.MultiplyExpression,
        SyntaxKind.SlashToken => SyntaxKind.DivideExpression,
        SyntaxKind.PlusToken => SyntaxKind.AddExpression,
        SyntaxKind.MinusToken => SyntaxKind.SubtractExpression,
        SyntaxKind.LessThanLessThanToken => SyntaxKind.LeftShiftExpression,
        SyntaxKind.GreaterThanGreaterThanToken => SyntaxKind.RightShiftExpression,
        SyntaxKind.EqualsEqualsToken => SyntaxKind.EqualsExpression,
        SyntaxKind.ExclamationEqualsToken => SyntaxKind.NotEqualsExpression,
        SyntaxKind.LessThanToken => SyntaxKind.LessThanExpression,
        SyntaxKind.LessThanEqualsToken => SyntaxKind.LessThanOrEqualExpression,
        SyntaxKind.GreaterThanToken => SyntaxKind.GreaterThanExpression,
        SyntaxKind.GreaterThanEqualsToken => SyntaxKind.GreaterThanOrEqualExpression,
        SyntaxKind.AmpersandToken => SyntaxKind.BitwiseAndExpression,
        SyntaxKind.AmpersandAmpersandToken => SyntaxKind.LogicalAndExpression,
        SyntaxKind.BarToken => SyntaxKind.BitwiseOrExpression,
        SyntaxKind.BarBarToken => SyntaxKind.LogicalOrExpression,
        SyntaxKind.CaretToken => SyntaxKind.ExclusiveOrExpression,
        SyntaxKind.HookHookToken => SyntaxKind.CoalesceExpression,
        _ => throw new UnreachableException($"Unexpected {nameof(SyntaxKind)}: '{operatorKind}'")
    };
}
