using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    public static DeclarationSyntax ParseGlobalDeclaration(SyntaxTokenStream stream) => stream.Current.SyntaxKind switch
    {
        // TODO: Support enum declarations.
        SyntaxKind.ModuleKeyword => ParseModuleDeclaration(stream),
        SyntaxKind.StructKeyword => ParseStructDeclaration(stream),
        _ => ParseVariableDeclaration(stream),
    };
}
