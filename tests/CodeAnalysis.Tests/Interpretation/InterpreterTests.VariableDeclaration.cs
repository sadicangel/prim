using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Tests.Interpretation;
public partial class InterpreterTests
{
    [Fact]
    public void Evaluates_VariableDeclaration_ArrayType()
    {
        var value = """
        a: [i32: 2] = [0, 0];
        """.Evaluate();
        Assert.True(value.Type is ArrayTypeSymbol);
    }

    [Fact]
    public void Evaluates_VariableDeclaration_LambdaType()
    {
        var value = """
        a: (x: i32, y: i32) -> i32 = x + y;
        """.Evaluate();
        Assert.True(value.Type is LambdaTypeSymbol);
    }

    [Fact]
    public void Evaluates_VariableDeclaration_OptionType()
    {
        var value = """
        a: ?i32 = null;
        """.Evaluate();
        Assert.True(value.Type is OptionTypeSymbol);
    }

    [Fact]
    public void Evaluates_VariableDeclaration_StructType()
    {
        var value = """
        a: i32 = 0;
        """.Evaluate();
        Assert.True(value.Type is StructTypeSymbol);
    }

    [Fact]
    public void Evaluates_VariableDeclaration_UnionType()
    {
        var value = """
        a: i32 | str = "";
        """.Evaluate();
        Assert.True(value.Type is UnionTypeSymbol);
    }

    [Fact]
    public void Evaluates_VariableDeclaration_with_implicit_type()
    {
        var value = """
        a:= "";
        """.Evaluate();
        Assert.Equal(PredefinedTypes.Str, value.Type.Name);
    }

    [Fact]
    public void Evaluates_VariableDeclaration_with_optional_value()
    {
        var value = """
        a: ?i32;
        """.Evaluate();
        var option = Assert.IsType<OptionValue>(value);
        Assert.Equal(PredefinedTypes.Unit, option.Value.Type.Name);
    }
}
