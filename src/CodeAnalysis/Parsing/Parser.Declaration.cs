using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    public static DeclarationSyntax ParseDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator) => iterator.Current.SyntaxKind switch
    {
        // TODO: Support enum declarations.
        SyntaxKind.ModuleKeyword => ParseModuleDeclaration(syntaxTree, iterator),
        SyntaxKind.StructKeyword => ParseStructDeclaration(syntaxTree, iterator),
        _ => ParseVariableDeclaration(syntaxTree, iterator),
    };
}
