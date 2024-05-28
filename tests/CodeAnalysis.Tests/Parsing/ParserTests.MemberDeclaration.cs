using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_MemberDeclaration()
    {
        var unit = SyntaxTree.Parse(new SourceText("""
            Container: struct : {
                member: i32 = 0;
            }
            """));
        var node = Assert.Single(unit.Root.SyntaxNodes);
        Assert.Empty(unit.Diagnostics);
        var decl = Assert.IsType<StructDeclarationSyntax>(node);
        var member = Assert.Single(decl.Members);
        Assert.Equal(SyntaxKind.MemberDeclaration, member.SyntaxKind);
    }
}
