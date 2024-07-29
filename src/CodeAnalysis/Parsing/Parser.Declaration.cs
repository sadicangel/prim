using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static DeclarationSyntax ParseDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        return iterator.Peek(2).SyntaxKind switch
        {
            // TODO: Support module declarations.
            // TODO: Support enum declarations.
            SyntaxKind.StructKeyword => ParseStructDeclaration(syntaxTree, iterator),
            _ => ParseVariableDeclaration(syntaxTree, iterator),
        };
    }
}
