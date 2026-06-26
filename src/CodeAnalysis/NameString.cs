using System.Collections;
using System.Text;
using CodeAnalysis.Syntax;

namespace CodeAnalysis;

public readonly record struct NameString(string FullName) : IEnumerable<string>
{
    public string Name { get; } = GetName(FullName);

    public bool IsEmpty => string.IsNullOrEmpty(FullName);

    public NameString(IEnumerable<string> parts) : this(string.Empty)
    {
        using var enumerator = parts.GetEnumerator();
        if (!enumerator.MoveNext()) return;
        Name = enumerator.Current;
        var fullName = new StringBuilder(Name);

        while (enumerator.MoveNext())
        {
            Name = enumerator.Current;
            fullName.Append(SyntaxFacts.NameSeparator).Append(Name);
        }

        FullName = fullName.ToString();
    }

    private static string GetName(string fullName)
    {
        var name = string.Empty;
        using var enumerator = GetEnumerator(fullName);
        while (enumerator.MoveNext()) name = enumerator.Current;
        return name;
    }

    private static IEnumerator<string> GetEnumerator(string fullName)
    {
        if (string.IsNullOrEmpty(fullName)) yield break;

        var start = 0;
        foreach (var (index, @char) in fullName.Index())
        {
            if (@char != SyntaxFacts.NameSeparator) continue;

            yield return fullName[start..index];
            start = index + 1;
        }

        yield return fullName[start..];
    }

    public IEnumerator<string> GetEnumerator() => GetEnumerator(FullName);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => FullName;
}
