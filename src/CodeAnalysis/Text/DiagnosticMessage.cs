using CodeAnalysis.Syntax;

namespace CodeAnalysis.Text;

public static class DiagnosticMessage
{
    public static string InvalidNumber(string value, PrimType type) => $"The value '{value}' is not a valid '{type}'";

    public static string InvalidCharacter(char character) => $"Invalid character input: '{character}'";

    public static string UnexpectedToken(TokenKind expected, TokenKind actual) => $"Unexpected token '{actual.GetDiagnosticDisplay()}'. Expected '{expected.GetDiagnosticDisplay()}'";

    public static string UndefinedUnaryOperator(Token @operator, PrimType operandType) => $"Unary operator '{@operator}' is not defined for type '{operandType}'";

    public static string UndefinedBinaryOperator(Token @operator, PrimType leftType, PrimType rightType) => $"Binary operator '{@operator}' is not defined for types '{leftType}' and '{rightType}'";

    public static string AmbiguousBinaryOperator(Token @operator, PrimType leftType, PrimType rightType) => $"Binary operator '{@operator}' is ambiguous on operands of type '{leftType}' and '{rightType}'";

    public static string UndefinedName(Token identifier) => $"'{identifier}' is undefined";

    public static string UndefinedType(string typeName) => $"Type '{typeName}' is undefined";

    public static string InvalidConversion(PrimType sourceType, PrimType destinationType) => $"Invalid conversion from type '{sourceType}' to '{destinationType}'";

    public static string InvalidImplicitConversion(PrimType sourceType, PrimType destinationType) => $"Invalid implicit conversion from type '{sourceType}' to '{destinationType}'. An explicit conversion exists (are you missing a cast?)";

    public static string RedundantConversion() => "Conversion is redundant";

    public static string Redeclaration(Token identifier, string type) => $"Redeclaration of {type} '{identifier}'";

    public static string ReadOnlyAssignment(string identifier) => $"Invalid assignment to read-only '{identifier}'";

    public static string UnterminatedString() => "Unterminated string literal";

    public static string UnterminatedComment() => "Unterminated comment";

    public static string InvalidArgumentCount(string functionName, int expectedCount, int actualCount) => $"Function '{functionName}' requires {expectedCount} arguments but was given {actualCount}";

    public static string InvalidArgumentType(string parameterName, PrimType expectedType, PrimType actualType) => $"Parameter '{parameterName}' cannot be converted from type '{actualType}' to '{expectedType}'";

    public static string InvalidExpressionType(PrimType actualType) => $"Invalid expression of type '{actualType}'";

    public static string InvalidExpressionType(PrimType expectedType, PrimType actualType) => $"Invalid expression of type '{actualType}'. Expected '{expectedType}'";

    public static string InvalidVariableType(PrimType expectedType, PrimType actualType) => $"Expected variable of type '{expectedType}'. Got '{actualType}'";

    public static string InvalidType() => $"Expected type definition";

    public static string InvalidSymbol(Token identifierToken, SymbolKind expectedKind, SymbolKind actualKind) => $"{actualKind} '{identifierToken.Text}' is not a '{expectedKind}'";

    public static string InvalidBreakOrContinue() => "No enclosing loop out of which to break or continue";

    public static string InvalidReturn() => "No enclosing function out of which to return";

    public static string InvalidReturnExpression(string functionName) => $"Since '{functionName}' returns void, a return keyword must not be followed by an expression";

    public static string InvalidReturnExpression(string functionName, PrimType expectedType) => $"Function '{functionName}' expects an expression of a type convertible to '{expectedType}'";

    public static string NotAllPathsReturn() => "Not all code paths return a value";

    public static string UnreachableCode() => "Unreachable code detected";
}
