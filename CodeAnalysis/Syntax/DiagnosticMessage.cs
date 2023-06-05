namespace CodeAnalysis.Syntax;

public static class DiagnosticMessage
{
    public static string InvalidNumber(string value, Type type) => $"The value '{value}' is not a valid '{type}'";

    public static string InvalidCharacter(char character) => $"Invalid character input: '{character}'";

    public static string UnexpectedToken(TokenKind expected, TokenKind actual) => $"Unexpected token '{actual.GetDiagnosticDisplay()}'. Expected '{expected.GetDiagnosticDisplay()}'";

    public static string UndefinedUnaryOperator(string @operator, Type operandType) => $"Unary operator '{@operator}' is not defined for type '{operandType.Name}'";

    public static string UndefinedBinaryOperator(string @operator, Type leftType, Type rightType) => $"Binary operator '{@operator}' is not defined for types '{leftType.Name}' and '{rightType.Name}'";

    public static string UndefinedName(string identifier) => $"'{identifier}' is undefined";

    public static string InvalidConversion(Type sourceType, Type destinationType) => $"Cannot convert from type '{sourceType}' to '{destinationType}'";

    public static string Redeclaration(string identifier) => $"Redeclaration of '{identifier}'";

    public static string ReadOnlyAssignment(string identifier) => $"Invalid assignment to read-only '{identifier}'";
}
