using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Tests.Interpretation;
public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_StructInitExpression()
    {
        var value = """
            Point: struct: { x: i32 = 0;  y: i32 = 0; }
            Point { .x = 10, .y = 10 }
            """.Evaluate();
        var objectValue = Assert.IsType<ObjectValue>(value);
        var x = objectValue.Members.Single(m => m.Key.Name == "x").Value.Value;
        var y = objectValue.Members.Single(m => m.Key.Name == "y").Value.Value;

        Assert.Equal(10, x);
        Assert.Equal(10, y);
    }
}
