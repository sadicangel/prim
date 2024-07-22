namespace CodeAnalysis.Tests.Interpretation;
public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_PropertyReference_get()
    {
        var value = """
            Point: struct: { x: i32 = 0;  y: i32 = 0; }
            p :: Point { .x = 10, .y = 10 }
            p.x
            """.Evaluate();
        Assert.Equal(10, value.Value);
    }

    [Fact]
    public void Evaluates_PropertyReference_set()
    {
        var value = """
            Point: struct: { x: i32 = 0;  y: i32 = 0; }
            p :: Point { .x = 10, .y = 10 }
            p.x = 20
            """.Evaluate();
        Assert.Equal(20, value.Value);
    }
}
