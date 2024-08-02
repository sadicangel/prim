using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Tests.Interpretation;

public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_LiteralExpression_I8()
    {
        var expected = new InstanceValue(_scope.I8, (sbyte)0);
        var actual = """
        0i8
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_U8()
    {
        var expected = new InstanceValue(_scope.U8, (byte)0);
        var actual = """
        0u8
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_I16()
    {
        var expected = new InstanceValue(_scope.I16, (short)0);
        var actual = """
        0i16
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_U16()
    {
        var expected = new InstanceValue(_scope.U16, (ushort)0);
        var actual = """
        0u16
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_I32()
    {
        var expected = new InstanceValue(_scope.I32, 0);
        var actual = """
        0i32
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_I32_without_suffix()
    {
        var expected = new InstanceValue(_scope.I32, 0);
        var actual = """
        0
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_U32()
    {
        var expected = new InstanceValue(_scope.U32, (uint)0);
        var actual = """
        0u32
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_I64()
    {
        var expected = new InstanceValue(_scope.I64, (long)0);
        var actual = """
        0i64
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_I64_without_suffix()
    {
        var expected = new InstanceValue(_scope.I64, (long)int.MaxValue + 1);
        var actual = """
        2147483648
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_U64()
    {
        var expected = new InstanceValue(_scope.U64, (ulong)0);
        var actual = """
        0u64
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_F16()
    {
        var expected = new InstanceValue(_scope.F16, (Half)0);
        var actual = """
        0f16
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_F32()
    {
        var expected = new InstanceValue(_scope.F32, (float)0);
        var actual = """
        0f32
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_F64()
    {
        var expected = new InstanceValue(_scope.F64, (double)0);
        var actual = """
        0f64
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_F64_without_suffix()
    {
        var expected = new InstanceValue(_scope.F64, (double)0);
        var actual = """
        0.0
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_Str()
    {
        var expected = new InstanceValue(_scope.Str, "");
        var actual = """
        ""
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_Bool()
    {
        var expected = new InstanceValue(_scope.Bool, true);
        var actual = """
        true
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Evaluates_LiteralExpression_Unit()
    {
        var expected = new InstanceValue(_scope.Unit, Unit.Value);
        var actual = """
        null
        """.Evaluate();
        Assert.Equal(expected, actual);
    }
}
