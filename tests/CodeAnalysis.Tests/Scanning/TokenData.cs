using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace CodeAnalysis.Tests.Scanning;

public readonly record struct TokenData(SyntaxKind SyntaxKind, string Text) : IXunitSerializable
{
    public void Deserialize(IXunitSerializationInfo info)
    {
        SetSyntaxKind(ref Unsafe.AsRef(in this), info.GetValue<SyntaxKind>(nameof(SyntaxKind)));
        SetText(ref Unsafe.AsRef(in this), info.GetValue<string>(nameof(Text)));

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_SyntaxKind")]
        extern static void SetSyntaxKind(ref TokenData tokenData, SyntaxKind syntaxKind);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Text")]
        extern static void SetText(ref TokenData tokenData, string text);
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        info.AddValue(nameof(SyntaxKind), SyntaxKind, typeof(SyntaxKind));
        info.AddValue(nameof(Text), Text, typeof(string));
    }
}
