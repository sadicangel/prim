namespace CodeAnalysis.Tests.Interpretation;
public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_MethodReference_with_a_single_match()
    {
        var value = """
        S: struct = {
            hello: () -> str = "Hello world";
        }

        s:: S {};
        s.hello()
        """.Evaluate();
        Assert.Equal("Hello world", value.Value);
    }

    [Fact]
    public void Evaluates_MethodReference_with_a_multiple_matches()
    {
        var value = """
        S: struct = {
            hello: () -> str = "Hello world";
            hello: (obj: any) -> str = "Hello " + obj;
        }

        s:: S {};
        s.hello("John")
        """.Evaluate();
        Assert.Equal("Hello John", value.Value);
    }
}
