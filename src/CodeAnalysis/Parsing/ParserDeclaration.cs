using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal static class ParserDeclaration
{
    extension(SyntaxTokenStream stream)
    {
        public DeclarationSyntax ParseGlobalDeclaration() => stream.Current.SyntaxKind switch
        {
            // TODO: Support enum declarations.
            SyntaxKind.ModuleKeyword => stream.ParseModuleDeclaration(),
            SyntaxKind.StructKeyword => stream.ParseStructDeclaration(),
            _ => stream.ParseVariableDeclaration(),
        };

        public DeclarationSyntax ParseLocalDeclaration() => stream.ParseVariableDeclaration();

        private ModuleDeclarationSyntax ParseModuleDeclaration()
        {
            var moduleKeyword = stream.Match(SyntaxKind.ModuleKeyword);
            var name = stream.ParseSimpleName();
            var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);

            return new ModuleDeclarationSyntax(moduleKeyword, name, semicolonToken);
        }

        private StructDeclarationSyntax ParseStructDeclaration()
        {
            var structKeyword = stream.Match(SyntaxKind.StructKeyword);
            var name = stream.ParseSimpleName();
            var braceOpenToken = stream.Match(SyntaxKind.BraceOpenToken);
            var properties = stream.ParseSyntaxList([SyntaxKind.BraceCloseToken], ParsePropertyDeclaration);
            var braceCloseToken = stream.Match(SyntaxKind.BraceCloseToken);

            return new StructDeclarationSyntax(
                structKeyword,
                name,
                braceOpenToken,
                properties,
                braceCloseToken);
        }

        private PropertyDeclarationSyntax ParsePropertyDeclaration()
        {
            var name = stream.ParseSimpleName();
            var typeClause = stream.ParseTypeClause(isOptional: false);
            var initClause = stream.ParseInitClause(isOptional: true);
            var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);

            return new PropertyDeclarationSyntax(
                name,
                typeClause,
                initClause,
                semicolonToken);
        }

        private VariableDeclarationSyntax ParseVariableDeclaration()
        {
            var bindingKeyword = stream.Match(SyntaxKind.LetKeyword, SyntaxKind.VarKeyword);
            var name = stream.ParseSimpleName();
            var typeClause = stream.ParseTypeClause(isOptional: true);
            var initClause = stream.ParseInitClause(isOptional: true);
            _ = stream.TryMatch(out var semicolonToken, SyntaxKind.SemicolonToken);

            return new VariableDeclarationSyntax(
                bindingKeyword,
                name,
                typeClause,
                initClause,
                semicolonToken);
        }

        private InitClauseSyntax? ParseInitClause([DoesNotReturnIf(false)] bool isOptional)
        {
            if (isOptional && stream.Current.SyntaxKind is not SyntaxKind.EqualsToken)
            {
                return null;
            }

            var equalsToken = stream.Match(SyntaxKind.EqualsToken);
            var expression = stream.ParseExpression();

            return new InitClauseSyntax(equalsToken, expression);
        }
    }
}
