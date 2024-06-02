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
        $"The value '{value}' is not a valid '{GetDisplayText(kind)}'";
    public static string UnterminatedComment() =>
        "Unterminated comment";
    public static string UnterminatedString() =>
        "Unterminated string literal";

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
    public static string ReportInvalidArrayLength() =>
        $"Invalid array length expression. Must be a constant i32 value";
    public static string SymbolRedeclaration(string symbolName) =>
        $"Redeclaration of symbol '{symbolName}'";
    public static string UndefinedType(string typeName) =>
        $"Undefined type '{typeName}'";
    public static string UndefinedTypeMember(string typeName, string memberName) =>
        $"'{typeName}' does not contain a definition for '{memberName}'";
    public static string ReportUndefinedSymbol(string symbolName) =>
        $"Undefined symbol '{symbolName}'";
    //public static string AmbiguousBinaryOperator(SyntaxToken @operator, PrimType leftType, PrimType rightType) => $"Binary operator '{@operator.Text}' is ambiguous on operands of type '{leftType.Name}' and '{rightType.Name}'";
    //public static string InvalidArgumentType(Parameter parameter, PrimType actualType) => $"Invalid expression of type '{actualType.Name}' provided for parameter '{parameter.Name}' of type '{parameter.Type.Name}'";
    //public static string UndefinedBinaryOperator(SyntaxToken @operator, PrimType leftType, PrimType rightType) => $"Binary operator '{@operator.Text}' is not defined for types '{leftType.Name}' and '{rightType.Name}'";
    //public static string InvalidExpressionType(PrimType expectedType, PrimType actualType) => $"Invalid expression of type '{actualType.Name}'. Expected '{expectedType.Name}'";
    //public static string InvalidImplicitConversion(PrimType sourceType, PrimType destinationType) => $"Invalid implicit conversion from type '{sourceType}' to '{destinationType}'. An explicit conversion exists (are you missing a cast?)";
    //public static string InvalidNumberOfArguments(FunctionType functionType, int actualNumberOfArguments) => $"Function '{functionType.Name}' requires {functionType.Parameters.Count} arguments but was given {actualNumberOfArguments}";
    //public static string InvalidSymbolType(PrimType expectedType, PrimType actualType) => $"Invalid symbol of type '{actualType.Name}'. Expected '{expectedType.Name}'";
    //public static string InvalidTypeConversion(PrimType sourceType, PrimType targetType) => $"Invalid conversion from type '{sourceType.Name}' to '{targetType.Name}'";
    //public static string SymbolReassignment(Symbol symbol) => $"Reassignment of read-only symbol '{symbol.Name}'";
    //public static string UndefinedUnaryOperator(SyntaxToken @operator, PrimType operandType) => $"Unary operator '{@operator.Text}' is not defined for type '{operandType.Name}'";

    //public static string RedundantConversion() => "Conversion is redundant";
    //public static string InvalidExpressionType(PrimType actualType) => $"Invalid expression of type '{actualType}'";
    //public static string InvalidSymbol(SyntaxToken identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => $"{actualKind} '{identifierToken.Text}' is not a '{expectedKind}'";
    //public static string InvalidBreakOrContinue() => "No enclosing loop out of which to break or continue";
    //public static string InvalidReturn() => "No enclosing function out of which to return";
    //public static string InvalidReturnExpression(string functionName) => $"Since '{functionName}' returns void, a return keyword must not be followed by an expression";
    //public static string InvalidReturnExpression(string functionName, PrimType expectedType) => $"Function '{functionName}' expects an expression of a type convertible to '{expectedType}'";
    //public static string NotAllPathsReturn() => "Not all code paths return a value";
    //public static string UnreachableCode() => "Unreachable code detected";
}
