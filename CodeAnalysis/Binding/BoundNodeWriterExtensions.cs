using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using System.CodeDom.Compiler;

namespace CodeAnalysis.Binding;
internal static class BoundNodeWriterExtensions
{
    public static void WriteTo(this BoundNode node, TextWriter writer)
    {
        if (writer is not IndentedTextWriter indentedWriter)
            indentedWriter = new IndentedTextWriter(writer);
        WriteTo(node, indentedWriter);
    }

    public static void WriteTo(this BoundNode node, IndentedTextWriter writer) => writer.WriteNode(node);

    private static void WriteNode(this IndentedTextWriter writer, BoundNode node)
    {
        switch (node.NodeKind)
        {
            case BoundNodeKind.LiteralExpression:
                writer.WriteNode((BoundLiteralExpression)node);
                break;
            case BoundNodeKind.UnaryExpression:
                writer.WriteNode((BoundUnaryExpression)node);
                break;
            case BoundNodeKind.BinaryExpression:
                writer.WriteNode((BoundBinaryExpression)node);
                break;
            case BoundNodeKind.SymbolExpression:
                writer.WriteNode((BoundSymbolExpression)node);
                break;
            case BoundNodeKind.AssignmentExpression:
                writer.WriteNode((BoundAssignmentExpression)node);
                break;
            case BoundNodeKind.CallExpression:
                writer.WriteNode((BoundCallExpression)node);
                break;
            case BoundNodeKind.NeverExpression:
                writer.WriteNode((BoundNeverExpression)node);
                break;
            case BoundNodeKind.IfExpression:
                writer.WriteNode((BoundIfExpression)node);
                break;
            case BoundNodeKind.BlockStatement:
                writer.WriteNode((BoundBlockStatement)node);
                break;
            case BoundNodeKind.VariableDeclaration:
                writer.WriteNode((BoundVariableDeclaration)node);
                break;
            case BoundNodeKind.ExpressionStatement:
                writer.WriteNode((BoundExpressionStatement)node);
                break;
            case BoundNodeKind.IfStatement:
                writer.WriteNode((BoundIfStatement)node);
                break;
            case BoundNodeKind.WhileStatement:
                writer.WriteNode((BoundWhileStatement)node);
                break;
            case BoundNodeKind.ForStatement:
                writer.WriteNode((BoundForStatement)node);
                break;
            case BoundNodeKind.ConvertExpression:
                writer.WriteNode((BoundConvertExpression)node);
                break;
            case BoundNodeKind.FunctionDeclaration:
                writer.WriteNode((BoundFunctionDeclaration)node);
                break;
            default:
                throw new NotSupportedException(node?.GetType()?.Name);
        }
    }

    private static void WritePunctuation(this IndentedTextWriter writer, TokenKind token)
    {
        writer.Write(token.GetText() ?? throw new InvalidOperationException($"Token {token} has no text"));
    }

    private static void WriteKeyword(this IndentedTextWriter writer, TokenKind token)
    {
        writer.Write(token.GetText() ?? throw new InvalidOperationException($"Token {token} has no text"));
    }

    private static void WriteIdentifier(this IndentedTextWriter writer, ReadOnlySpan<char> text)
    {
        writer.Write(text);
    }

    private static void WriteType(this IndentedTextWriter writer, ReadOnlySpan<char> text)
    {
        writer.Write(text);
    }

    private static void WriteNode(this IndentedTextWriter writer, BoundBlockStatement node)
    {
        writer.WritePunctuation(TokenKind.OpenBrace);
        writer.WriteLine();
        writer.Indent++;
        foreach (var statement in node.Statements)
            writer.WriteNode(statement);
        writer.Indent--;
        writer.WritePunctuation(TokenKind.CloseBrace);
        writer.WriteLine();
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundVariableDeclaration node)
    {
        writer.WritePunctuation(node.Variable.IsReadOnly ? TokenKind.Let : TokenKind.Var);
        writer.WriteIdentifier(node.Variable.Name);
        writer.WritePunctuation(TokenKind.Colon);
        writer.Write(" ");
        writer.WriteType(node.Variable.Type.Name);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.Equal);
        writer.Write(" ");
        writer.WriteNode(node.Expression);
        writer.WritePunctuation(TokenKind.Semicolon);
        writer.WriteLine();
    }
    private static void WriteNode(this IndentedTextWriter writer, ParameterSymbol parameter)
    {
        writer.WriteIdentifier(parameter.Name);
        writer.WritePunctuation(TokenKind.Colon);
        writer.WriteType(parameter.Type.Name);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundFunctionDeclaration node)
    {
        writer.WriteKeyword(TokenKind.Let);
        writer.WriteIdentifier(node.Function.Name);
        writer.WritePunctuation(TokenKind.Colon);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.OpenParenthesis);
        if (node.Function.Parameters.Count > 0)
        {
            writer.Write(node.Function.Parameters[0]);
            foreach (var parameter in node.Function.Parameters.Skip(1))
            {
                writer.WritePunctuation(TokenKind.Comma);
                writer.Write(" ");
                writer.WriteNode(parameter);
            }
        }
        writer.WritePunctuation(TokenKind.CloseParenthesis);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.Arrow);
        writer.Write(" ");
        writer.WriteType(node.Function.Type.Name);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.Equal);
        writer.WriteNestedStatement(node.Body);

    }
    private static void WriteNode(this IndentedTextWriter writer, BoundExpressionStatement node)
    {
        writer.WriteNode(node.Expression);
        writer.WritePunctuation(TokenKind.Semicolon);
        writer.WriteLine();
    }
    private static void WriteNestedStatement(this IndentedTextWriter writer, BoundStatement node)
    {
        var needsIndentation = node is not BoundBlockStatement;
        if (needsIndentation)
            writer.Indent++;
        writer.WriteNode(node);
        if (needsIndentation)
            writer.Indent--;
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundIfStatement node)
    {
        writer.WriteKeyword(TokenKind.If);
        writer.Write(" ");
        writer.WriteNode(node.Condition);
        writer.WriteLine();
        writer.WriteNestedStatement(node.Then);
        if (node.HasElseClause)
        {
            writer.WriteKeyword(TokenKind.Else);
            writer.WriteLine();
            writer.WriteNestedStatement(node.Else);
        }
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundWhileStatement node)
    {
        writer.WriteKeyword(TokenKind.While);
        writer.Write(" ");
        writer.WriteNode(node.Condition);
        writer.WriteLine();
        writer.WriteNestedStatement(node.Body);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundForStatement node)
    {
        writer.WriteKeyword(TokenKind.For);
        writer.Write(" ");
        writer.WriteKeyword(TokenKind.Var);
        writer.Write(" ");
        writer.WriteIdentifier(node.Variable.Name);
        writer.Write(" ");
        writer.WriteKeyword(TokenKind.In);
        writer.WriteNode(node.LowerBound);
        writer.WritePunctuation(TokenKind.Range);
        writer.WriteNode(node.UpperBound);
        writer.WriteLine();
        writer.WriteNestedStatement(node.Body);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundNeverExpression node)
    {
        writer.Write("<never>");
    }
    private static void WriteNestedExpression(this IndentedTextWriter writer, int parentPrecedence, BoundExpression node)
    {
        switch (node)
        {
            case BoundUnaryExpression expression:
                writer.WriteNestedExpression(parentPrecedence, expression.Operator.TokenKind.GetUnaryOperatorPrecedence(), expression);
                break;
            case BoundBinaryExpression expression:
                writer.WriteNestedExpression(parentPrecedence, expression.Operator.TokenKind.GetBinaryOperatorPrecedence(), expression);
                break;
            default:
                writer.WriteNode(node);
                break;
        }
    }
    private static void WriteNestedExpression(this IndentedTextWriter writer, int parentPrecedence, int currentPrecedence, BoundExpression node)
    {
        var needsParenthesis = parentPrecedence >= currentPrecedence;
        if (needsParenthesis)
            writer.WritePunctuation(TokenKind.OpenParenthesis);
        writer.WriteNode(node);
        if (needsParenthesis)
            writer.WritePunctuation(TokenKind.CloseParenthesis);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundUnaryExpression node)
    {
        writer.WritePunctuation(node.Operator.TokenKind);
        writer.WriteNestedExpression(node.Operator.TokenKind.GetUnaryOperatorPrecedence(), node.Operand);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundBinaryExpression node)
    {
        var precedence = node.Operator.TokenKind.GetBinaryOperatorPrecedence();
        writer.WriteNestedExpression(precedence, node.Left);
        writer.Write(" ");
        writer.WritePunctuation(node.Operator.TokenKind);
        writer.Write(" ");
        writer.WriteNestedExpression(precedence, node.Right);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundLiteralExpression node)
    {
        switch (node.Type)
        {
            default:
                writer.Write(node.Value);
                break;
        }
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundSymbolExpression node)
    {
        writer.WriteIdentifier(node.Symbol.Name);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundAssignmentExpression node)
    {
        writer.WriteIdentifier(node.Variable.Name);
        writer.Write(" ");
        writer.WritePunctuation(TokenKind.Equal);
        writer.Write(" ");
        writer.WriteNode(node.Expression);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundIfExpression node)
    {
        writer.WriteKeyword(TokenKind.If);
        writer.Write(" ");
        writer.WriteNode(node.Condition);
        writer.Write(" ");
        writer.WriteNode(node.Then);
        writer.WriteKeyword(TokenKind.Else);
        writer.Write(" ");
        writer.WriteNode(node.Else);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundCallExpression node)
    {
        writer.WriteIdentifier(node.Function.Name);
        writer.WritePunctuation(TokenKind.OpenParenthesis);
        if (node.Arguments.Count > 0)
        {
            writer.WriteNode(node.Arguments[0]);
            foreach (var argument in node.Arguments.Skip(1))
            {
                writer.WritePunctuation(TokenKind.Comma);
                writer.Write(" ");
                writer.WriteNode(argument);
            }
        }
        writer.WritePunctuation(TokenKind.CloseParenthesis);
    }
    private static void WriteNode(this IndentedTextWriter writer, BoundConvertExpression node)
    {
        writer.WriteNode(node.Expression);
        writer.Write(" ");
        writer.WriteKeyword(TokenKind.As);
        writer.Write(" ");
        writer.WriteType(node.Type.Name);
    }
}
