using CodeAnalysis.Syntax;

namespace CodeAnalysis.Diagnostics;

internal static class DiagnosticMessage
{
    private static string? GetDisplayText(SyntaxKind syntaxKind) =>
        SyntaxFacts.GetText(syntaxKind) ?? syntaxKind.ToString();

    // Scanning error messages.
    public static string InvalidCharacter(char character) =>
        $"Invalid character input: '{character}'";
    public static string InvalidSyntaxValue(string value, SyntaxKind kind) =>
        $"'{value}' is not a valid '{GetDisplayText(kind)}'";
    public static string UnterminatedComment() =>
        $"Unterminated comment";
    public static string UnterminatedString() =>
        $"Unterminated string literal";

    // Parsing error messages.
    public static string ExpectedTypeDefinition() =>
        $"Expected type definition";
    public static string InvalidLocationForFunctionDefinition() =>
        $"Invalid location for function definition";
    public static string InvalidLocationForTypeDefinition() =>
        $"Invalid location for type definition";
    public static string UnexpectedToken(SyntaxKind expected, SyntaxKind actual) =>
        $"Unexpected token '{GetDisplayText(actual)}'. Expected '{GetDisplayText(expected)}'";

    // Binding error messages.
    public static string AmbiguousBinaryOperator(SyntaxToken @operator, string leftTypeName, string rightTypeName) =>
        $"Binary operator '{@operator.Text}' is ambiguous on operands of type '{leftTypeName}' and '{rightTypeName}'";
    public static string AmbiguousUnaryOperator(SyntaxToken @operator, string operandTypeName) =>
        $"Unary operator '{@operator.Text}' is ambiguous on operand of type '{operandTypeName}'";
    public static string InvalidArgumentListLength(string functionName, int expectedLength, int actualLength) =>
        $"Function '{functionName}' expects {expectedLength} arguments but was given {actualLength}";
    public static string InvalidArrayLength() =>
        $"Invalid array length expression. Must be a constant {SyntaxFacts.GetText(SyntaxKind.I32Keyword)} value";
    public static string InvalidConversion(string sourceTypeName, string targetTypeName) =>
        $"Invalid conversion from type '{sourceTypeName}' to '{targetTypeName}'";
    public static string InvalidExpressionType(string expectedTypeName, string actualTypeName) =>
        $"Invalid expression of type '{actualTypeName}'. Expected '{expectedTypeName}'";
    public static string InvalidFunctionSymbol() =>
        $"Not a function";
    public static string InvalidImplicitConversion(string sourceTypeName, string targeTypeName) =>
        $"Invalid implicit conversion from type '{sourceTypeName}' to '{targeTypeName}'. An explicit conversion exists (are you missing a cast?)";
    public static string MutableGlobalDeclaration(string declarationKind) =>
        $"Invalid global '{declarationKind}' declaration. Must be readonly";
    public static string ReadOnlyAssignment(string symbolName) =>
        $"Invalid assignment of '{symbolName}'. It is read-only";
    public static string RedundantConversion() =>
        "Conversion is redundant";
    public static string SymbolRedeclaration(string symbolName) =>
        $"Redeclaration of symbol '{symbolName}'";
    public static string UndefinedBinaryOperator(SyntaxToken @operator, string leftTypeName, string rightTypeName) =>
        $"Binary operator '{@operator.Text}' is not defined for types '{leftTypeName}' and '{rightTypeName}'";
    public static string UndefinedType(string typeName) =>
        $"Undefined type '{typeName}'";
    public static string UndefinedTypeMember(string typeName, string memberName) =>
        $"'{typeName}' does not contain a definition for '{memberName}'";
    public static string UndefinedSymbol(string symbolName) =>
        $"Undefined symbol '{symbolName}'";
    public static string UndefinedUnaryOperator(SyntaxToken @operator, string operandTypeName) =>
        $"Undefined unary operator '{@operator.Text}' for type '{operandTypeName}'";




    //public static string InvalidExpressionType(PrimType actualType) => $"Invalid expression of type '{actualType}'";
    //public static string InvalidSymbol(SyntaxToken identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => $"{actualKind} '{identifierToken.Text}' is not a '{expectedKind}'";
    //public static string InvalidBreakOrContinue() => "No enclosing loop out of which to break or continue";
    //public static string InvalidReturn() => "No enclosing function out of which to return";
    //public static string InvalidReturnExpression(string functionName) => $"Since '{functionName}' returns void, a return keyword must not be followed by an expression";
    //public static string NotAllPathsReturn() => "Not all code paths return a value";
    //public static string UnreachableCode() => "Unreachable code detected";
}
