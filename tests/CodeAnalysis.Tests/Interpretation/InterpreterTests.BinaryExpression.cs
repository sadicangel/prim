using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Tests.Interpretation;
public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_AddExpression()
    {
        var expected = new InstanceValue(_scope.I32, 5);
        var actual = """
        3 + 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_SubtractExpression()
    {
        var expected = new InstanceValue(_scope.I32, 1);
        var actual = """
        3 - 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_MultiplyExpression()
    {
        var expected = new InstanceValue(_scope.I32, 6);
        var actual = """
        3 * 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_DivideExpression()
    {
        var expected = new InstanceValue(_scope.I32, 1);
        var actual = """
        3 / 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_ModuloExpression()
    {
        var expected = new InstanceValue(_scope.I32, 1);
        var actual = """
        3 % 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_PowerExpression()
    {
        var expected = new InstanceValue(_scope.I32, 9);
        var actual = """
        3 ** 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LeftShiftExpression()
    {
        var expected = new InstanceValue(_scope.I32, 6);
        var actual = """
        3 << 1
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_RightShiftExpression()
    {
        var expected = new InstanceValue(_scope.I32, 1);
        var actual = """
        3 >> 1
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LogicalOrExpression()
    {
        var expected = new InstanceValue(_scope.Bool, true);
        var actual = """
        true || false
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LogicalAndExpression()
    {
        var expected = new InstanceValue(_scope.Bool, false);
        var actual = """
        true && false
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_BitwiseOrExpression()
    {
        var expected = new InstanceValue(_scope.I32, 3);
        var actual = """
        1 | 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_BitwiseAndExpression()
    {
        var expected = new InstanceValue(_scope.I32, 2);
        var actual = """
        2 & 3
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_ExclusiveOrExpression()
    {
        var expected = new InstanceValue(_scope.I32, 5);
        var actual = """
        2 + 3
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_EqualsExpression()
    {
        var expected = new InstanceValue(_scope.Bool, true);
        var actual = """
        2 == 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_NotEqualsExpression()
    {
        var expected = new InstanceValue(_scope.Bool, false);
        var actual = """
        2 != 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LessThanExpression()
    {
        var expected = new InstanceValue(_scope.Bool, false);
        var actual = """
        2 < 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LessThanOrEqualExpression()
    {
        var expected = new InstanceValue(_scope.Bool, true);
        var actual = """
        2 <= 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_GreaterThanExpression()
    {
        var expected = new InstanceValue(_scope.Bool, false);
        var actual = """
        2 > 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_GreaterThanOrEqualExpression()
    {
        var expected = new InstanceValue(_scope.Bool, true);
        var actual = """
        2 >= 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_CoalesceExpression()
    {
        var expected = new InstanceValue(_scope.I32, 2);
        var actual = """
        (null as ?i32) ?? 2
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
}
