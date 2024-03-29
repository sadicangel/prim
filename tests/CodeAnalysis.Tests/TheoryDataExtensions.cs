﻿namespace CodeAnalysis.Tests;

internal static class TheoryDataExtensions
{
    public static TheoryData<TSource> ToTheoryData<TSource>(this IEnumerable<TSource> elements)
    {
        var data = new TheoryData<TSource>();
        foreach (var t in elements)
            data.Add(t);
        return data;
    }
    public static TheoryData<T1> ToTheoryData<TSource, T1>(this IEnumerable<TSource> elements, Func<TSource, T1> selector)
    {
        var data = new TheoryData<T1>();
        foreach (var t1 in elements.Select(selector))
            data.Add(t1);
        return data;
    }

    public static TheoryData<T1, T2> ToTheoryData<TSource, T1, T2>(this IEnumerable<TSource> elements, Func<TSource, (T1, T2)> selector)
    {
        var data = new TheoryData<T1, T2>();
        foreach (var (t1, t2) in elements.Select(selector))
            data.Add(t1, t2);
        return data;
    }

    public static TheoryData<T1, T2, T3> ToTheoryData<TSource, T1, T2, T3>(this IEnumerable<TSource> elements, Func<TSource, (T1, T2, T3)> selector)
    {
        var data = new TheoryData<T1, T2, T3>();
        foreach (var (t1, t2, t3) in elements.Select(selector))
            data.Add(t1, t2, t3);
        return data;
    }
}
