using CodeAnalysis.Interpretation;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Tests.Interpretation;

public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_LiteralExpression_I32()
    {
        var expected = new LiteralValue(GlobalEvaluatedScope.Instance.I32, 0);
        var actual = """
        0
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_U32()
    {
        var expected = new LiteralValue(GlobalEvaluatedScope.Instance.U32, 0u);
        var actual = """
        0u
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_I64()
    {
        var expected = new LiteralValue(GlobalEvaluatedScope.Instance.I64, 0L);
        var actual = """
        0l
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_U64()
    {
        var expected = new LiteralValue(GlobalEvaluatedScope.Instance.U64, 0ul);
        var actual = """
        0ul
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_F32()
    {
        var expected = new LiteralValue(GlobalEvaluatedScope.Instance.F32, 0f);
        var actual = """
        0f
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_F64()
    {
        var expected = new LiteralValue(GlobalEvaluatedScope.Instance.F64, 0d);
        var actual = """
        0d
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_Str()
    {
        var expected = PrimValue.EmptyStr;
        var actual = """
        ""
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_Bool()
    {
        var expected = PrimValue.True;
        var actual = """
        true
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_Unit()
    {
        var expected = PrimValue.Unit;
        var actual = """
        null
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
}
