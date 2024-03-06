using System.Diagnostics;
using Dax.Tokenizer;

namespace Dax.Vpax.Obfuscator.Extensions;

internal static class StringExtensions
{
    public static bool IsDaxKeyword(this string value)
        => DaxKeywords.Contains(value ?? throw new ArgumentNullException(nameof(value)));

    public static bool IsFullyQualifiedColumnName(this string value)
        => value.TrimEnd().EndsWith("]") && value.IndexOf('[') > 0;

    public static (string table, string column) GetFullyQualifiedColumnNameParts(this string value)
    {
        Debug.Assert(IsFullyQualifiedColumnName(value));

        var openIndex = value.IndexOf('[');
        var closeIndex = value.LastIndexOf(']');
        var table = value.Substring(0, openIndex);
        var column = value.Substring(openIndex + 1, closeIndex - openIndex - 1);

        return (table, column);
    }

    public static string EscapeDax(this string value, int tokenType)
    {
        // See <TOKEN_TYPE>_action() methods in Dax.Tokenizer.DaxLexer class

        switch (tokenType)
        {
            case DaxToken.TABLE:
            case DaxToken.UNTERMINATED_TABLEREF:
                return value.Replace("'", "''");

            case DaxToken.STRING_LITERAL:
                return value.Replace("\"", "\"\"");

            case DaxToken.COLUMN_OR_MEASURE:
            case DaxToken.UNTERMINATED_COLREF:
                return value.Replace("]", "]]");
        }

        return value;
    }

    private static readonly HashSet<string> DaxKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        // TOFIX: get keywords from tokenizer instead of hardcoding
        "MEASURE",
        "COLUMN",
        "TABLE",
        "CALCULATIONGROUP",
        "CALCULATIONITEM",
        "DETAILROWS",
        "DEFINE",
        "EVALUATE",
        "ORDER",
        "BY",
        "START",
        "AT",
        "RETURN",
        "VAR",
        "NOT",
        "IN",
        "ASC",
        "DESC",
        "SKIP",
        "DENSE",
        "BLANK",
        "BLANKS",
        "SECOND",
        "MINUTE",
        "HOUR",
        "DAY",
        "MONTH",
        "QUARTER",
        "YEAR",
        "WEEK",
        "BOTH",
        "NONE",
        "ONEWAY",
        "ONEWAY_RIGHTFILTERSLEFT",
        "ONEWAY_LEFTFILTERSRIGHT",
        "CURRENCY",
        "INTEGER",
        "DOUBLE",
        "STRING",
        "BOOLEAN",
        "DATETIME",
        "VARIANT",
        "TEXT",
        "ALPHABETICAL",
        "KEEP",
        "FIRST",
        "LAST",
        "DEFAULT",
        "TRUE",
        "FALSE",
        "ABS",
        "REL",
    };
}
