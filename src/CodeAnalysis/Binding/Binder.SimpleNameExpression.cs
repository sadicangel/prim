using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindSimpleNameExpression(SimpleNameExpressionSyntax syntax, Context context)
    {
        if (syntax.IdentifierToken.IsSynthetic)
        {
            // Diagnostic already reported since identifier was not parsed.
            return new BoundNeverExpression(syntax, context.BoundScope.Never);
        }

        var symbolName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not Symbol symbol)
        {
            context.Diagnostics.ReportUndefinedSymbol(syntax.Location, symbolName);
            return new BoundNeverExpression(syntax, context.BoundScope.Never);
        }

        return new BoundLocalReference(syntax, symbol, symbol.Type);
    }
}
