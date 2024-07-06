namespace Odin.CodeGen.Abstractions.Utils;

public static class FilterTypeUtils
{
    public static EFilterType GetFilterType(string str)
    {
        var type = str switch
        {
            "Any" => EFilterType.Any,
            "All" => EFilterType.All,
            "Not" => EFilterType.Not,
            "Has" => EFilterType.Has,
            "NotHas" => EFilterType.NotHas,
            "Added" => EFilterType.Added,
            "Removed" => EFilterType.Removed,
            "AnyChanges" => EFilterType.AnyChanges,
            _ => EFilterType.Unknown
        };

        return type;
    }
}