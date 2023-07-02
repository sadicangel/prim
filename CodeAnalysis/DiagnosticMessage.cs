using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis;

public static class DiagnosticMessage
{
    public static string InvalidNumber(string value, TypeSymbol type) => $"The value '{value}' is not a valid '{type}'";

    public static string InvalidCharacter(char character) => $"Invalid character input: '{character}'";

    public static string UnexpectedToken(TokenKind expected, TokenKind actual) => $"Unexpected token '{actual.GetDiagnosticDisplay()}'. Expected '{expected.GetDiagnosticDisplay()}'";

    public static string UndefinedUnaryOperator(string @operator, TypeSymbol operandType) => $"Unary operator '{@operator}' is not defined for type '{operandType.Name}'";

    public static string UndefinedBinaryOperator(string @operator, TypeSymbol leftType, TypeSymbol rightType) => $"Binary operator '{@operator}' is not defined for types '{leftType.Name}' and '{rightType.Name}'";

    public static string UndefinedName(string identifier) => $"'{identifier}' is undefined";

    public static string UndefinedType(string typeName) => $"Type '{typeName}' is undefined";

    public static string InvalidConversion(TypeSymbol sourceType, TypeSymbol destinationType) => $"Invalid conversion from type '{sourceType}' to '{destinationType}'";

    public static string InvalidImplicitConversion(TypeSymbol sourceType, TypeSymbol destinationType) => $"Invalid implicit conversion from type '{sourceType}' to '{destinationType}'. An explicit conversion exists (are you missing a cast?)";

    public static string Redeclaration(string identifier, string type) => $"Redeclaration of {type} '{identifier}'";

    public static string ReadOnlyAssignment(string identifier) => $"Invalid assignment to read-only '{identifier}'";

    public static string UnterminatedString() => "Unterminated string literal";

    public static string InvalidArgumentCount(string functionName, int expectedCount, int actualCount) => $"Function '{functionName}' requires {expectedCount} arguments but was given {actualCount}";

    public static string InvalidArgumentType(string parameterName, TypeSymbol expectedType, TypeSymbol actualType) => $"Parameter '{parameterName}' cannot be converted from type '{actualType}' to '{expectedType}'";

    public static string InvalidExpressionType(TypeSymbol actualType) => $"Invalid expression of type '{actualType}'";

    public static string InvalidExpressionType(TypeSymbol expectedType, TypeSymbol actualType) => $"Invalid expression of type '{actualType}'. Expected '{expectedType}'";

    public static string InvalidVariableType(TypeSymbol expectedType, TypeSymbol actualType) => $"Expected variable of type '{expectedType}'. Got '{actualType}'";

    public static string InvalidSymbol(Token identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => $"{actualKind} '{identifierToken.Text}' is not a '{expectedKind}'";

    public static string NotSupported(string @object) => $"Not supported: {@object}";
    public static string InvalidBreakOrContinue() => "No enclosing loop out of which to break or continue";
}
