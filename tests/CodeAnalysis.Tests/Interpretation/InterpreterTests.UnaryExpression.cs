﻿using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Tests.Interpretation;
public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_UnaryPlusExpression()
    {
        var expected = new InstanceValue(_scope.I32, 2);
        var actual = """
        +2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_UnaryMinusExpression()
    {
        var expected = new InstanceValue(_scope.I32, -2);
        var actual = """
        -2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_OnesComplementExpression()
    {
        var expected = new InstanceValue(_scope.I32, -3);
        var actual = """
        ~2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_NotExpression()
    {
        var expected = new InstanceValue(_scope.Bool, false);
        var actual = """
        !true
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
}
