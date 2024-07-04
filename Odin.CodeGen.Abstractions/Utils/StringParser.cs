namespace Odin.CodeGen.Abstractions.Utils;

public static class StringParser
{
    public static Substring ParseRecursiveGroups(string str, char start = '(', char end = ')')
    {
        var from = str.IndexOf(start);

        var deep = 0;
        for (var i = from + 1; i < str.Length; i++)
        {
            var c = str[i];
            if (c == start)
            {
                deep++;
                continue;
            }


            if (c == end)
            {
                if (deep == 0)
                {
                    var text = str.Substring(from, i - from + 1);
                    text = text.Substring(1);
                    text = text.Substring(0, text.Length - 1);

                    return new Substring()
                    {
                        InnerText = text,
                        StartIndex = from,
                        EndIndex = i + 1
                    };
                }

                deep--;
            }
        }

        return new Substring()
        {
            InnerText = string.Empty,
            StartIndex = from,
            EndIndex = str.Length
        };
    }
}