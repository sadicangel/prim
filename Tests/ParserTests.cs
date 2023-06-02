﻿using CodeAnalysis.Syntax;

namespace Tests;
public sealed class ParserTests
{
    [Theory]
    [MemberData(nameof(GetBinaryOperatorsPairsData))]
    public void Parser_BinaryExpression_HonorsPrecedence(TokenKind op1, TokenKind op2)
    {
        var op1Precedence = op1.GetBinaryOperatorPrecendence();
        var op2Precedence = op2.GetBinaryOperatorPrecendence();

        var op1Text = op1.GetText();
        var op2Text = op2.GetText();

        var text = $"a {op1Text} b {op2Text} c";
        var expr = SyntaxTree.Parse(text).Root;

        if (op1Precedence >= op2Precedence)
        {
            using var e = new AssertingEnumerator(expr);
            e.AssertNode(NodeKind.BinaryExpression);
            e.AssertNode(NodeKind.BinaryExpression);
            e.AssertNode(NodeKind.NameExpression);
            e.AssertToken(TokenKind.Identifier, "a");
            e.AssertToken(op1, op1Text!);
            e.AssertNode(NodeKind.NameExpression);
            e.AssertToken(TokenKind.Identifier, "b");
            e.AssertToken(op2, op2Text!);
            e.AssertNode(NodeKind.NameExpression);
            e.AssertToken(TokenKind.Identifier, "c");
        }
        else
        {
            using var e = new AssertingEnumerator(expr);
            e.AssertNode(NodeKind.BinaryExpression);
            e.AssertNode(NodeKind.NameExpression);
            e.AssertToken(TokenKind.Identifier, "a");
            e.AssertToken(op1, op1Text!);
            e.AssertNode(NodeKind.BinaryExpression);
            e.AssertNode(NodeKind.NameExpression);
            e.AssertToken(TokenKind.Identifier, "b");
            e.AssertToken(op2, op2Text!);
            e.AssertNode(NodeKind.NameExpression);
            e.AssertToken(TokenKind.Identifier, "c");
        }
    }

    public static IEnumerable<object[]> GetBinaryOperatorsPairsData()
    {
        foreach (var op1 in SyntaxFacts.GetBinaryOperators())
            foreach (var op2 in SyntaxFacts.GetBinaryOperators())
                yield return new object[] { op1, op2 };
    }


    [Theory]
    [MemberData(nameof(GetUnaryOperatorsPairsData))]
    public void Parser_UnaryExpression_HonorsPrecedence(TokenKind unaryKind, TokenKind binaryKind)
    {
        var unaryPrecedence = unaryKind.GetUnaryOperatorPrecendence();
        var binaryPrecedence = binaryKind.GetBinaryOperatorPrecendence();

        var unaryText = unaryKind.GetText();
        var binaryText = binaryKind.GetText();

        var text = $"{unaryText} a {binaryText} b";
        var expr = SyntaxTree.Parse(text).Root;

        if (unaryPrecedence >= binaryPrecedence)
        {
            using var e = new AssertingEnumerator(expr);
            e.AssertNode(NodeKind.BinaryExpression);
            e.AssertNode(NodeKind.UnaryExpression);
            e.AssertToken(unaryKind, unaryText!);
            e.AssertNode(NodeKind.NameExpression);
            e.AssertToken(TokenKind.Identifier, "a");
            e.AssertToken(binaryKind, binaryText!);
            e.AssertNode(NodeKind.NameExpression);
            e.AssertToken(TokenKind.Identifier, "b");
        }
        else
        {
            using var e = new AssertingEnumerator(expr);
            e.AssertNode(NodeKind.UnaryExpression);
            e.AssertToken(unaryKind, unaryText!);
            e.AssertNode(NodeKind.BinaryExpression);
            e.AssertNode(NodeKind.NameExpression);
            e.AssertToken(TokenKind.Identifier, "a");
            e.AssertToken(binaryKind, binaryText!);
            e.AssertNode(NodeKind.NameExpression);
            e.AssertToken(TokenKind.Identifier, "b");
        }
    }

    public static IEnumerable<object[]> GetUnaryOperatorsPairsData()
    {
        foreach (var unary in SyntaxFacts.GetUnaryOperators())
            foreach (var binary in SyntaxFacts.GetBinaryOperators())
                yield return new object[] { unary, binary };
    }
}