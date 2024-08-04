using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Tests.Interpretation;
public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_StructDeclaration()
    {
        var value = """
            Point: struct = { x: i32 = 0;  y: i32 = 0; }
            """.Evaluate();
        Assert.True(value is StructValue);
    }
}
