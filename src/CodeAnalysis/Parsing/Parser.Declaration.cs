using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    public static DeclarationSyntax ParseGlobalDeclaration(SyntaxIterator iterator) => iterator.Current.SyntaxKind switch
    {
        // TODO: Support enum declarations.
        SyntaxKind.ModuleKeyword => ParseModuleDeclaration(iterator),
        SyntaxKind.StructKeyword => ParseStructDeclaration(iterator),
        _ => ParseVariableDeclaration(iterator),
    };
}
