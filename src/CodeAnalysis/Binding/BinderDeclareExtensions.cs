using System.Collections.Immutable;
using System.Diagnostics;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Binding;

internal static class BinderDeclareExtensions
{
    extension(ModuleBinder binder)
    {
        public void DeclareSymbols(ImmutableArray<SyntaxTree> syntaxTrees)
        {
            foreach (var compilationUnit in syntaxTrees.Select(x => x.CompilationUnit))
            {
                binder.DeclareCompilationUnitTypes(compilationUnit);
            }

            foreach (var compilationUnit in syntaxTrees.Select(x => x.CompilationUnit))
            {
                binder.DeclareCompilationUnitGlobals(compilationUnit);
            }
        }

        private void DeclareCompilationUnitTypes(CompilationUnitSyntax compilationUnit)
        {
            if (compilationUnit.Module is not null)
            {
                var module = binder.DeclareModule(compilationUnit.Module);
                binder = new ModuleBinder(module, binder);
            }

            foreach (var structSyntax in compilationUnit.Declarations.Select(x => x.Declaration).OfType<StructDeclarationSyntax>())
            {
                binder.DeclareStruct(structSyntax);
            }
        }

        private void DeclareCompilationUnitGlobals(CompilationUnitSyntax compilationUnit)
        {
            if (compilationUnit.Module is not null)
            {
                if (!binder.TryLookup<ModuleSymbol>(compilationUnit.Module.Name.FullName, out var module))
                {
                    throw new UnreachableException($"Missing {nameof(ModuleSymbol)} '{compilationUnit.Module.Name.FullName}'");
                }

                binder = new ModuleBinder(module, binder);
            }

            foreach (var declaration in compilationUnit.Declarations.Select(x => x.Declaration))
            {
                switch (declaration.SyntaxKind)
                {
                    case SyntaxKind.StructDeclaration:
                        {
                            binder.DeclareStructProperties((StructDeclarationSyntax)declaration);
                            break;
                        }

                    case SyntaxKind.VariableDeclaration:
                        {
                            binder.DeclareVariable((VariableDeclarationSyntax)declaration);
                            break;
                        }

                    default:
                        throw new UnreachableException($"Unexpected syntax '{declaration.GetType()}'");
                }
            }
        }

        private ModuleSymbol DeclareModule(ModuleDeclarationSyntax syntax)
        {
            var module = new ModuleSymbol(syntax, syntax.Name.FullName, binder.Module);
            _ = binder.TryDeclare(module);
            return module;
        }

        private StructSymbol DeclareStruct(StructDeclarationSyntax syntax)
        {
            var @struct = new StructSymbol(syntax, syntax.Name.FullName, binder.Module);

            if (!binder.TryDeclare(@struct))
            {
                binder.ReportSymbolRedeclaration(syntax.SourceSpan, @struct.Name);
            }

            return @struct;
        }

        private void DeclareStructProperties(StructDeclarationSyntax syntax)
        {
            if (!binder.TryLookup<StructSymbol>(syntax.Name.FullName, out var @struct))
            {
                // TODO: Should this be a diagnostics instead?
                throw new UnreachableException($"Missing {nameof(StructSymbol)} '{syntax.Name.FullName}'");
            }

            foreach (var propertySyntax in syntax.Properties)
            {
                var propertyType = binder.BindType(propertySyntax.TypeClause.Type);
                var property = new PropertySymbol(propertySyntax, propertySyntax.Name.FullName, propertyType, @struct, Modifiers.None);
                if (!@struct.TryDeclare(property))
                {
                    binder.ReportSymbolRedeclaration(propertySyntax.SourceSpan, property.Name);
                }
            }
        }

        private void DeclareVariable(VariableDeclarationSyntax syntax)
        {
            var variableType = syntax.TypeClause is not null ? binder.BindType(syntax.TypeClause.Type) : binder.Module.Never;
            if (variableType == binder.Module.Never)
            {
                binder.ReportInvalidImplicitType(syntax.SourceSpan, variableType.FullName);
            }

            var modifiers = syntax.BindingKeyword.SyntaxKind is SyntaxKind.LetKeyword ? Modifiers.ReadOnly : Modifiers.None;

            var variable = new VariableSymbol(syntax, syntax.Name.FullName, variableType, binder.Module, modifiers);
            if (!binder.TryDeclare(variable))
            {
                binder.ReportSymbolRedeclaration(syntax.SourceSpan, variable.Name);
            }
        }
    }
}
