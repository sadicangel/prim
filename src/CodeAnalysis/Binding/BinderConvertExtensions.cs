using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.References;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;

internal static class BinderConvertExtensions
{
    extension(Binder binder)
    {
        public BoundExpression Convert(BoundExpression expression, TypeSymbol targetType, bool isExplicit = false)
        {
            if (expression.Type.MapsToNever || expression.Type == targetType)
            {
                return expression;
            }

            if (!TryGetConversion(expression.Type, targetType, out var conversion))
            {
                binder.ReportInvalidConversion(expression.Syntax.SourceSpan, targetType.Name, expression.Type.Name);
                return new BoundNeverExpression(expression.Syntax, binder.Module.Never);
            }

            if (conversion is null)
            {
                return expression;
            }

            if (!isExplicit)
            {
                if (conversion.ConversionKind is ConversionKind.Implicit)
                {
                    return CreateConversionExpression(expression, conversion);
                }

                binder.ReportInvalidImplicitConversion(expression.Syntax.SourceSpan, expression.Type.Name, targetType.Name);
                return new BoundNeverExpression(expression.Syntax, binder.Module.Never);
            }

            if (conversion.ConversionKind is ConversionKind.Implicit)
            {
                binder.ReportRedundantConversion(expression.Syntax.SourceSpan);
            }

            return CreateConversionExpression(expression, conversion);
        }
    }

    private static bool TryGetConversion(TypeSymbol source, TypeSymbol target, out ConversionSymbol? conversion)
    {
        if (source == target || target.IsAny || (target.MapsToAny && source.SymbolKind == target.SymbolKind))
        {
            conversion = null;
            return true;
        }

        if (source.MapsToNever || target.MapsToNever)
        {
            conversion = null;
            return false;
        }

        conversion = source.FindConversion(source, target) ?? target.FindConversion(source, target);
        return conversion is not null;
    }

    extension(TypeSymbol containingType)
    {
        private ConversionSymbol? FindConversion(TypeSymbol source, TypeSymbol target)
        {
            var binder = new TypeBinder(containingType);
            foreach (var conversionName in EnumerateCandidateNames(source, target))
            {
                if (binder.TryLookup<ConversionSymbol>(conversionName, out var conversion))
                {
                    return conversion;
                }
            }

            return null;

            static IEnumerable<string> EnumerateCandidateNames(TypeSymbol source, TypeSymbol target)
            {
                yield return ConversionSymbol.GetConversionName(source, target);
            }
        }
    }

    private static BoundCallExpression CreateConversionExpression(BoundExpression expression, ConversionSymbol conversion) =>
        new(
            expression.Syntax,
            new BoundConversionReference(expression.Syntax, conversion),
            ImmutableArray.Create(expression),
            conversion.LambdaType.ReturnType);
}
