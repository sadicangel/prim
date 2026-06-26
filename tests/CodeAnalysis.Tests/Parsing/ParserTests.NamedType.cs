namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_NamedType_simple_name()
    {
        var declaration = ParseGlobalDeclaration("value: Point = 0;");
        var type = Assert.IsType<NamedTypeSyntax>(declaration.Type);

        Assert.IsType<SimpleNameSyntax>(type.Name);
        Assert.Equal("Point", type.Name.FullName);
    }

    [Fact]
    public void Parse_NamedType_qualified_name()
    {
        var declaration = ParseGlobalDeclaration("value: math.Point = 0;");
        var type = Assert.IsType<NamedTypeSyntax>(declaration.Type);

        Assert.IsType<QualifiedNameSyntax>(type.Name);
        Assert.Equal("math.Point", type.Name.FullName);
    }
}
