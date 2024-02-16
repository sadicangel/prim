namespace CodeAnalysis.Types.Members;

public abstract record class Member(
    string Name,
    PrimType Type
);