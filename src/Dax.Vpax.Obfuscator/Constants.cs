namespace Dax.Vpax.Obfuscator;

internal static class Constants
{
    /// <summary>
    /// Reserved characters that are preserved during obfuscation
    /// </summary>
    public static readonly HashSet<char> ReservedChars = new()
    {
        '-',  // single-line comment char
        '/',  // multi-line comment char
        '*',  // multi-line comment char
        '\n', // line feed char e.g. in multi-line comments
        '\r', // carriage return char e.g. in multi-line comments
    };

    /// <summary>
    /// CALENDAR() [Date] column
    /// </summary>
    public const string DaxKeyword_Date = "DATE";

    /// <summary>
    /// ''[Value] or table constructor { } [ValueN] column when there are multiple columns
    /// </summary>
    public const string DaxKeyword_Value = "VALUE";

    /// <summary>
    /// Contains DAX keywords that are not obfuscable.
    /// </summary>
    /// <remarks>
    /// A DAX keyword is not obfuscable when the keyword is also a valid unquoted table name.
    /// </remarks>
    public static readonly HashSet<string> DaxKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        DaxKeyword_Date,
        DaxKeyword_Value,

        // From DAX lexer grammar. Interval (Z__FIRSTKEYWORD__ .. Z__LASTKEYWORD__) excluding DDL keywords.
        "ABS",
        "ALPHABETICAL",
        "ANCHORED",
        "ANYREF",
        "ANYVAL",
        "BLANKS",
        "BOOLEAN",
        "BOTH",
        "CALENDARREF",
        "COLUMNREF",
        "COLUMNS",
        "CURRENCY",
        "DATATYPE",
        "DATETIME",
        "DAY",
        "DECIMAL",
        "DENSIFY",
        "DEFAULT",
        "DOUBLE",
        "EXPR",
        "EXTENDING",
        "FIRST",
        "HOUR",
        "INT64",
        "INTEGER",
        "KEEP",
        "LAST",
        "MEASUREREF",
        "MINUTE",
        "MONTH",
        "NONE",
        "NUMERIC",
        "ONEWAY_LEFTFILTERSRIGHT",
        "ONEWAY_RIGHTFILTERSLEFT",
        "ONEWAY",
        "PRECISE",
        "QUARTER",
        "REL",
        "ROWS",
        "SCALAR",
        "SECOND",
        "STRING",
        "TABLEREF",
        "TEXT",
        "VAL",
        "VARIANT",
        "WEEK",
        "YEAR",
        "TOTAL",
    };
}
