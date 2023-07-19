﻿using System.Runtime.CompilerServices;

namespace CodeAnalysis;

internal static class PathInfo
{
    public static string ProjectPath { get; }
    public static string SolutionPath { get; }

    static PathInfo()
    {
        var thisClassPath = GetProjectPath();
        ProjectPath = Directory.GetParent(thisClassPath)!.FullName;
        SolutionPath = Directory.GetParent(ProjectPath)!.FullName;
    }

    private static string GetProjectPath([CallerFilePath] string callerFilePath = "") => callerFilePath;
}