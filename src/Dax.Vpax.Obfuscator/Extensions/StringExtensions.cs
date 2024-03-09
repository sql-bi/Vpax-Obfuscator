using Dax.Tokenizer;

namespace Dax.Vpax.Obfuscator.Extensions;

internal static class StringExtensions
{
    public static bool IsBracketed(this string value) => value.StartsWith("[") && value.EndsWith("]");
    public static bool IsQuoted(this string value) => value.StartsWith("'") && value.EndsWith("'");
    public static bool IsDaxKeyword(this string value) => DaxKeywords.Contains(value ?? throw new ArgumentNullException(nameof(value)));

    public static string Unquote(this string value)
    {
        if (!IsQuoted(value)) throw new InvalidOperationException($"Invalid format. '{value}' is not a quoted string.");
        return value.Substring(1, value.Length - 2);
    }

    public static string Unbracket(this string value)
    {
        if (!IsBracketed(value)) throw new InvalidCastException($"Invalid format. '{value}' is not a bracketed string.");
        return value.Substring(1, value.Length - 2);
    }

    public static bool TryGetTableAndColumnNames(this string value, out string table, out string column)
    {
        //value = value.UnescapeDax(DaxToken.STRING_LITERAL);

        var endsWithBracket = value.TrimEnd().EndsWith("]");
        if (!endsWithBracket) goto unqualified_column_name;

        var startsWithQuote = value.TrimStart().StartsWith("'");
        if (startsWithQuote)
        {
            var quoteOpenIndex = value.IndexOf('\'');
            var quoteCloseIndex = value.Replace("''", "__").IndexOf('\'', quoteOpenIndex + 1);
            if (quoteCloseIndex == -1) goto unqualified_column_name;
            table = value.Substring(0, quoteCloseIndex + 1).Trim().Unquote();
            column = value.Substring(quoteCloseIndex + 1);

            var columnTrimmed = column.Trim();
            if (!columnTrimmed.IsBracketed()) goto unqualified_column_name;
            column = columnTrimmed.Unbracket();
            if (column.Replace("]]", "__").Contains("]")) goto unqualified_column_name;
        }
        else
        {
            var bracketOpenIndex = value.Replace("[[", "__").IndexOf('[');
            if (bracketOpenIndex == -1) goto unqualified_column_name;
            table = value.Substring(0, bracketOpenIndex);
            column = value.Substring(bracketOpenIndex).TrimEnd().Unbracket();

            if (column.Replace("]]", "__").Contains("]")) goto unqualified_column_name;
            if (IsInvalidUnquotedTable(table)) goto unqualified_column_name;
        }

        //table = table.UnescapeDax(DaxToken.TABLE);
        //column = column.UnescapeDax(DaxToken.COLUMN_OR_MEASURE);
        return true;

    unqualified_column_name:
        table = null!;
        column = value;
        return false;

        static bool IsInvalidUnquotedTable(string name)
        {
            name = name.Trim();
            return name.Length == 0
                || "0123456789".Contains(name[0]) // starts with a digit
                || name.Any((c) => c != '_' && !DaxTextObfuscator.CharSet.Contains(c));
        }
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

    public static string UnescapeDax(this string value, int tokenType)
    {
        // See <TOKEN_TYPE>_action() methods in Dax.Tokenizer.DaxLexer class

        switch (tokenType)
        {
            case DaxToken.TABLE:
            case DaxToken.UNTERMINATED_TABLEREF:
                return value.Replace("''", "'");

            case DaxToken.STRING_LITERAL:
                return value.Replace("\"\"", "\"");

            case DaxToken.COLUMN_OR_MEASURE:
            case DaxToken.UNTERMINATED_COLREF:
                return value.Replace("]]", "]");
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
