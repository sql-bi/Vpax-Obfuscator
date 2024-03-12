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
    /// See also DMV $SYSTEM.DISCOVER_KEYWORDS
    /// </summary>
    public static readonly HashSet<string> DaxKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        DaxKeyword_Date,
        DaxKeyword_Value,

        // TOFIX: get keywords from tokenizer instead of hardcoding
        // Z__FIRSTKEYWORD__
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
        // Z__LASTKEYWORD__
    };
}
