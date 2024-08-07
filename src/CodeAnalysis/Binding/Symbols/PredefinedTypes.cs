﻿using System.Reflection;

namespace CodeAnalysis.Binding.Symbols;

internal static class PredefinedTypes
{
    public const string Type = "type";
    public const string Any = "any";
    public const string Err = "err";
    public const string Unknown = "unknown";
    public const string Never = "never";
    public const string Unit = "unit";
    public const string Str = "str";
    public const string Bool = "bool";
    public const string I8 = "i8";
    public const string I16 = "i16";
    public const string I32 = "i32";
    public const string I64 = "i64";
    public const string Isz = "isz";
    public const string U8 = "u8";
    public const string U16 = "u16";
    public const string U32 = "u32";
    public const string U64 = "u64";
    public const string Usz = "usz";
    public const string F16 = "f16";
    public const string F32 = "f32";
    public const string F64 = "f64";

    public static ReadOnlyList<string> All { get; } = new(typeof(PredefinedTypes)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.IsLiteral)
        .Select(f => (string)f.GetValue(null)!)
        .ToArray());
}
