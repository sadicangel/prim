using CodeAnalysis.Symbols;

namespace CodeAnalysis.Syntax;

public static class DiagnosticMessage
{
    public static string InvalidNumber(string value, TypeSymbol type) => $"The value '{value}' is not a valid '{type}'";

    public static string InvalidCharacter(char character) => $"Invalid character input: '{character}'";

    public static string UnexpectedToken(TokenKind expected, TokenKind actual) => $"Unexpected token '{actual.GetDiagnosticDisplay()}'. Expected '{expected.GetDiagnosticDisplay()}'";

    public static string UndefinedUnaryOperator(string @operator, TypeSymbol operandType) => $"Unary operator '{@operator}' is not defined for type '{operandType.Name}'";

    public static string UndefinedBinaryOperator(string @operator, TypeSymbol leftType, TypeSymbol rightType) => $"Binary operator '{@operator}' is not defined for types '{leftType.Name}' and '{rightType.Name}'";

    public static string UndefinedName(string identifier) => $"'{identifier}' is undefined";

    public static string InvalidConversion(TypeSymbol sourceType, TypeSymbol destinationType) => $"Cannot convert from type '{sourceType}' to '{destinationType}'";

    public static string Redeclaration(string identifier) => $"Redeclaration of '{identifier}'";

    public static string ReadOnlyAssignment(string identifier) => $"Invalid assignment to read-only '{identifier}'";
    public static string UnterminatedString() => "Unterminated string literal";
    public static string InvalidArgumentCount(string functionName, int expectedCount, int actualCount) => $"Function '{functionName}' expected {expectedCount} arguments. Got {actualCount}";
    public static string InvalidArgumentType(string parameterName, TypeSymbol expectedType, TypeSymbol actualType) => $"Parameter '{parameterName}' cannot be converted from type '{actualType}' to '{expectedType}'";
    internal static string InvalidExpressionType(TypeSymbol type) => $"Invalid expression of type '{type}'";
}
