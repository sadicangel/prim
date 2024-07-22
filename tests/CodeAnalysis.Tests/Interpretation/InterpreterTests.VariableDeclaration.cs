using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Tests.Interpretation;
public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_VariableDeclaration_ArrayType()
    {
        var value = """
        a: [i32: 2]: [0, 0];
        a
        """.Evaluate();
        Assert.True(value.Type is ArrayTypeSymbol);
    }

    [Fact]
    public void Evaluates_VariableDeclaration_LambdaType()
    {
        var value = """
        a: (x: i32, y: i32) -> i32: x + y;
        a
        """.Evaluate();
        Assert.True(value.Type is LambdaTypeSymbol);
    }

    [Fact]
    public void Evaluates_VariableDeclaration_OptionType()
    {
        var value = """
        a: ?i32: null;
        a
        """.Evaluate();
        Assert.True(value.Type is OptionTypeSymbol);
    }

    [Fact]
    public void Evaluates_VariableDeclaration_StructType()
    {
        var value = """
        a: i32: 0;
        a
        """.Evaluate();
        Assert.True(value.Type is StructTypeSymbol);
    }

    [Fact]
    public void Evaluates_VariableDeclaration_UnionType()
    {
        var value = """
        a: i32 | str: "";
        a
        """.Evaluate();
        Assert.True(value.Type is UnionTypeSymbol);
    }
}
