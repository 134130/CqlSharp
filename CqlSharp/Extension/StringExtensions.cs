using System.Text;

namespace CqlSharp.Extension;

internal static class StringExtensions
{
    public static string Replace(this string str, char oldChar, string newValue, char escapeChar)
    {
        var firstIndex = str.IndexOf(oldChar);

        if (firstIndex < 0)
            return str;

        var sb = new StringBuilder(str[..firstIndex]);
        var sbIndex = firstIndex;

        for (var i = firstIndex; i < str.Length; i++)
        {
            if (str[i] != oldChar)
                continue;

            if (i == 0)
            {
                sb.Append(newValue);
                sbIndex = i + 1;
                continue;
            }

            if (str[i - 1] == escapeChar)
                continue;

            sb.Append(str[sbIndex..i]);
            sb.Append(newValue);
            sbIndex = i + 1;
        }

        sb.Append(str[sbIndex..]);

        return sb.ToString();
    }
}