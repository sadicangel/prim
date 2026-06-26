namespace CodeAnalysis.Tests.Parsing;

public partial class ParserTests
{
    [Fact]
    public void Parse_MemberAccessExpression()
    {
        var node = Assert.IsType<MemberAccessExpressionSyntax>(ParseExpression("a.b"));
        var receiver = Assert.IsType<NameExpressionSyntax>(node.Receiver);

        Assert.Equal("a", receiver.Name.FullName);
        Assert.Equal("b", node.MemberName.FullName);
    }

    [Fact]
    public void Parse_MemberAccessExpression_multiple_levels()
    {
        var node = Assert.IsType<MemberAccessExpressionSyntax>(ParseExpression("a.b.c.d"));
        Assert.Equal("d", node.MemberName.FullName);

        var abc = Assert.IsType<MemberAccessExpressionSyntax>(node.Receiver);
        Assert.Equal("c", abc.MemberName.FullName);

        var ab = Assert.IsType<MemberAccessExpressionSyntax>(abc.Receiver);
        Assert.Equal("b", ab.MemberName.FullName);

        var a = Assert.IsType<NameExpressionSyntax>(ab.Receiver);
        Assert.Equal("a", a.Name.FullName);
    }

    [Fact]
    public void Parse_ObjectInitializerExpression_with_property_access_type_name()
    {
        var node = Assert.IsType<ObjectInitializerExpressionSyntax>(ParseExpression("math.Point2D { x = 0, y = 1 }"));
        var typeName = Assert.IsType<MemberAccessExpressionSyntax>(node.TypeName);
        var receiver = Assert.IsType<NameExpressionSyntax>(typeName.Receiver);

        Assert.Equal("math", receiver.Name.FullName);
        Assert.Equal("Point2D", typeName.MemberName.FullName);
        Assert.Equal(2, node.Properties.Count);
        Assert.Equal("x", node.Properties[0].PropertyName.FullName);
        Assert.Equal("y", node.Properties[1].PropertyName.FullName);
    }

    [Fact]
    public void Parse_MemberAccessExpression_after_invocation_and_element_access()
    {
        var node = Assert.IsType<MemberAccessExpressionSyntax>(ParseExpression("a.b(c)[i].d"));
        Assert.Equal("d", node.MemberName.FullName);

        var elementAccess = Assert.IsType<ElementAccessExpressionSyntax>(node.Receiver);
        var invocation = Assert.IsType<InvocationExpressionSyntax>(elementAccess.Receiver);
        var callee = Assert.IsType<MemberAccessExpressionSyntax>(invocation.Callee);
        var receiver = Assert.IsType<NameExpressionSyntax>(callee.Receiver);

        Assert.Equal("a", receiver.Name.FullName);
        Assert.Equal("b", callee.MemberName.FullName);
        Assert.Single(invocation.Arguments);
        Assert.IsType<NameExpressionSyntax>(invocation.Arguments[0]);
        Assert.IsType<NameExpressionSyntax>(elementAccess.Index);
    }
}
