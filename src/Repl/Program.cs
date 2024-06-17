using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

const string Code = """
name: str = "test";
""";

var syntaxTree = SyntaxTree.Parse(new SourceText(Code));
