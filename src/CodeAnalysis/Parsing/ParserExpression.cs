using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal static class ParserExpression
{
    extension(SyntaxTokenStream stream)
    {
        public ExpressionSyntax ParseExpression()
        {
            var expression = stream.ParseBinaryExpression();
            return expression;
        }
    }
}
