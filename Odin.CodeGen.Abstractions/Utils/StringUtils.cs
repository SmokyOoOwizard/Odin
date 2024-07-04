namespace Odin.CodeGen.Abstractions.Utils;

public static class StringUtils
{
    public static string RemoveComments(this string str)
    {
        str = RemoveMultiLineComments(str);
        str = RemoveLineComments(str);
        
        return str;
    }

    public static string RemoveLineComments(string str)
    {
        do
        {
            var startIndex = str.IndexOf("//", StringComparison.Ordinal);
            if (startIndex == -1)
                break;

            bool lineEndingType = false;
            var endIndex = str.IndexOf("\r\n", startIndex, StringComparison.Ordinal);
            if (endIndex == -1)
            {
                endIndex = str.IndexOf("\n", startIndex, StringComparison.Ordinal);
                lineEndingType = true;
            }

            if (endIndex == -1)
            {
                str = str.Substring(0, 0);
                break;
            }

            str = str.Remove(startIndex, endIndex - startIndex + (lineEndingType ? 1 : 2));

        } while (true);

        return str;
    }
    
    public static string RemoveMultiLineComments(string str)
    {
        do
        {
            var startIndex = str.IndexOf("/*", StringComparison.Ordinal);
            if (startIndex == -1)
                break;
            
            var endIndex = str.IndexOf("*/", startIndex, StringComparison.Ordinal);

            if (endIndex == -1)
            {
                str = str.Substring(0, 0);
                break;
            }

            str = str.Remove(startIndex, endIndex - startIndex + 2);

        } while (true);

        return str;
    }
}