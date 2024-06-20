using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

internal static partial class Binder
{
    private static readonly List<DeclarationSyntax> s_emptyDeclarationList = [];

    public static BoundCompilationUnit Bind(BoundTree boundTree, BoundScope boundScope)
    {
        var compilationUnit = boundTree.SyntaxTree.CompilationUnit;

        if (compilationUnit.SyntaxTree.Diagnostics.HasErrorDiagnostics)
        {
            return new BoundCompilationUnit(compilationUnit, []);
        }

        var context = new BinderContext(boundTree, boundScope);

        var declarations = compilationUnit.SyntaxNodes
            .OfType<DeclarationSyntax>()
            .GroupBy(d => d.SyntaxKind)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.StructDeclaration, s_emptyDeclarationList))
            DeclareStruct((StructDeclarationSyntax)declaration, context, isForwardDeclarationOnly: true);

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.FunctionDeclaration, s_emptyDeclarationList))
            DeclareFunction((FunctionDeclarationSyntax)declaration, context);

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.VariableDeclaration, s_emptyDeclarationList))
            DeclareVariable((VariableDeclarationSyntax)declaration, context);

        foreach (var declaration in declarations.GetValueOrDefault(SyntaxKind.StructDeclaration, s_emptyDeclarationList))
            DeclareStruct((StructDeclarationSyntax)declaration, context);

        return BindCompilationUnit(compilationUnit, context);
    }
}
