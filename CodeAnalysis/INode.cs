﻿using CodeAnalysis.Text;

namespace CodeAnalysis;

public interface INode
{
    TextSpan Span { get; }

    void WriteTo(TextWriter writer, string indent = "", bool isLast = true);

    IEnumerable<INode> GetChildren();
}