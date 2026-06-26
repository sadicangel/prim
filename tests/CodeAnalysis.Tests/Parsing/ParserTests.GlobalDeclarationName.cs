namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_GlobalModuleDeclaration_simple_name()
    {
        var declaration = ParseGlobalDeclaration("a :: module;");

        Assert.IsType<SimpleNameSyntax>(declaration.Name);
        Assert.Equal("a", declaration.Name.FullName);
    }

    [Fact]
    public void Parse_GlobalModuleDeclaration_qualified_name()
    {
        var declaration = ParseGlobalDeclaration("a.b :: module;");

        Assert.IsType<QualifiedNameSyntax>(declaration.Name);
        Assert.Equal("a.b", declaration.Name.FullName);
    }

    [Fact]
    public void Parse_GlobalTypeDeclaration_qualified_name()
    {
        var declaration = ParseGlobalDeclaration("a.b :: type { };");

        Assert.IsType<QualifiedNameSyntax>(declaration.Name);
        Assert.IsType<TypeExpressionSyntax>(declaration.Initializer);
    }

    [Fact]
    public void Parse_GlobalVariableDeclaration_qualified_name()
    {
        var declaration = ParseGlobalDeclaration("a.b := 1;");

        Assert.IsType<QualifiedNameSyntax>(declaration.Name);
        Assert.Equal("a.b", declaration.Name.FullName);
    }

    [Fact]
    public void Parse_LocalDeclaration_does_not_accept_qualified_name()
    {
        var block = Assert.IsType<BlockExpressionSyntax>(ParseExpressionWithDiagnostics("{ a.b := 1; }", out _));

        Assert.DoesNotContain(block.Items, item => item is LocalDeclarationSyntax);
    }
}
