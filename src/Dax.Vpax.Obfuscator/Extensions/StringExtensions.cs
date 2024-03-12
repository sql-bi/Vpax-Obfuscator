using Dax.Tokenizer;

namespace Dax.Vpax.Obfuscator.Extensions;

internal static class StringExtensions
{
    public static bool StartsWithDigit(this string value) => "0123456789".Contains(value[0]); // Unckecked null/lenght, null or empty strings are not expected
    public static bool IsBracketed(this string value) => value.StartsWith("[") && value.EndsWith("]");
    public static bool IsQuoted(this string value) => value.StartsWith("'") && value.EndsWith("'");
    public static bool IsEmptyOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value ?? throw new ArgumentNullException(nameof(value)));
    public static bool IsDaxKeyword(this string value) => Constants.DaxKeywords.Contains(value ?? throw new ArgumentNullException(nameof(value)));

    public static bool IsDaxReservedName(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        // CALENDAR() [Date] extension column
        if (value.Equals(Constants.DaxKeyword_Date, StringComparison.OrdinalIgnoreCase))
            return true;

        if (value.StartsWith(Constants.DaxKeyword_Value, StringComparison.OrdinalIgnoreCase))
        {
            // ''[Value] column OR table constructor { } column when there is only one column
            if (value.Length == Constants.DaxKeyword_Value.Length)
                return true;

            // Table constructor { } [ValueN] column when there are multiple columns
            if (int.TryParse(value.Substring(Constants.DaxKeyword_Value.Length), out _))
                return true;
        }

        return false;
    }

    public static string Unquote(this string value)
    {
        if (!IsQuoted(value)) throw new InvalidOperationException($"Invalid format. '{value}' is not a quoted string.");
        return value.Substring(1, value.Length - 2);
    }

    public static string Unbracket(this string value)
    {
        if (!IsBracketed(value)) throw new InvalidOperationException($"Invalid format. '{value}' is not a bracketed string.");
        return value.Substring(1, value.Length - 2);
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

    public static bool TryGetTableAndColumnNames(this string value, out string table, out string column)
    {
        var endsWithBracket = value.TrimEnd().EndsWith("]");
        if (!endsWithBracket) goto unqualified_column_name;

        var startsWithQuote = value.TrimStart().StartsWith("'");
        if (startsWithQuote)
        {
            var quoteOpenIndex = value.IndexOf('\'');
            var quoteCloseIndex = value.Replace("''", "__").IndexOf('\'', quoteOpenIndex + 1);
            if (quoteCloseIndex == -1) goto unqualified_column_name;
            table = value.Substring(0, quoteCloseIndex + 1).Trim();
            column = value.Substring(quoteCloseIndex + 1);

            var columnTrimmed = column.Trim();
            if (!columnTrimmed.IsBracketed()) goto unqualified_column_name;
            column = columnTrimmed.Unbracket();
            if (column.Replace("]]", "__").Contains(']')) goto unqualified_column_name;
        }
        else
        {
            var bracketOpenIndex = value.Replace("[[", "__").IndexOf('[');
            if (bracketOpenIndex == -1) goto unqualified_column_name;
            table = value.Substring(0, bracketOpenIndex);
            column = value.Substring(bracketOpenIndex).TrimEnd().Unbracket();

            if (column.Replace("]]", "__").Contains(']')) goto unqualified_column_name;
            if (IsInvalidUnquotedTable(table)) goto unqualified_column_name;
        }

        return true;

    unqualified_column_name:
        table = null!;
        column = value;
        return false;

        static bool IsInvalidUnquotedTable(string name)
        {
            name = name.Trim();
            return name.Length == 0
                || name.StartsWithDigit()
                || name.Any((c) => c != '_' && !DaxTextObfuscator.CharSet.Contains(c));
        }
    }
}
