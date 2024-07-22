using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Tests.Interpretation;
public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_MethodGroup_to_first_method()
    {
        var value = """
        S: struct = {
            hello: () -> str = "Hello world";
            hello: (obj: any) -> str = "Hello " + obj;
        }

        s:: S {};
        s.hello
        """.Evaluate();
        var expectedType = new LambdaTypeSymbol([], PredefinedSymbols.Str);
        Assert.Equal(expectedType, value.Type);
    }
}
