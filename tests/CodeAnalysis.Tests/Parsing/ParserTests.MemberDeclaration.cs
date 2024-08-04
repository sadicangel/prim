using CodeAnalysis.Syntax.Expressions.Declarations;

namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_MemberDeclaration()
    {
        var tree = SyntaxTree.Parse(new SourceText("""
            Container: struct : {
                member: i32 = 0;
            }
            """));
        var node = Assert.Single(tree.CompilationUnit.SyntaxNodes);
        Assert.Empty(tree.Diagnostics);
        var decl = Assert.IsType<StructDeclarationSyntax>(node);
        var member = Assert.Single(decl.Members);
        Assert.Equal(SyntaxKind.PropertyDeclaration, member.SyntaxKind);
    }
}
