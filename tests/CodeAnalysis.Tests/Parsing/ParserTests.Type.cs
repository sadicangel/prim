using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Tests.Parsing;
partial class ParserTests
{
    public static TheoryData<string> GetPredefinedTypeNames() => new(PredefinedTypeNames.All);

    [Theory]
    [MemberData(nameof(GetPredefinedTypeNames))]
    public void Parse_Type_PredefinedType(string typeName)
    {
        var unit = SyntaxTree.Parse(new SourceText($"a: {typeName}: undefined;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        var decl = Assert.IsType<VariableDeclarationSyntax>(node);
        Assert.Equal(SyntaxKind.PredefinedType, decl.Type.SyntaxKind);
    }

    [Fact]
    public void Parse_Type_NamedTyped()
    {
        var unit = SyntaxTree.Parse(new SourceText($"a: SomeType: undefined;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        var decl = Assert.IsType<VariableDeclarationSyntax>(node);
        Assert.Equal(SyntaxKind.NamedType, decl.Type.SyntaxKind);
    }

    [Fact]
    public void Parse_Type_OptionType()
    {
        var unit = SyntaxTree.Parse(new SourceText($"a: ?SomeType: undefined;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        var decl = Assert.IsType<VariableDeclarationSyntax>(node);
        Assert.Equal(SyntaxKind.OptionType, decl.Type.SyntaxKind);
    }

    [Fact]
    public void Parse_Type_UnionType()
    {
        var unit = SyntaxTree.Parse(new SourceText($"a: i32 | ?i64 | SomeType: undefined;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        var decl = Assert.IsType<VariableDeclarationSyntax>(node);
        Assert.Equal(SyntaxKind.UnionType, decl.Type.SyntaxKind);
    }

    [Fact]
    public void Parse_Type_ArrayType()
    {
        var unit = SyntaxTree.Parse(new SourceText($"a: [u8: 42]: undefined;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        var decl = Assert.IsType<VariableDeclarationSyntax>(node);
        Assert.Equal(SyntaxKind.ArrayType, decl.Type.SyntaxKind);
    }

    [Fact]
    public void Parse_Type_FunctionType()
    {
        var unit = SyntaxTree.Parse(new SourceText($"a: (i: u8) -> i32: i;"));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        var decl = Assert.IsType<FunctionDeclarationSyntax>(node);
        Assert.Equal(SyntaxKind.FunctionType, decl.Type.SyntaxKind);
    }
}
