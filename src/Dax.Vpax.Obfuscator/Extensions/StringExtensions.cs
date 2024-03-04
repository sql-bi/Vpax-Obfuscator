using System.Diagnostics;
using Dax.Tokenizer;

namespace Dax.Vpax.Obfuscator.Extensions;

internal static class StringExtensions
{
    public static bool IsFullyQualifiedColumnName(this string value)
        => value.EndsWith("]") && value.IndexOf('[') > 0;

    public static (string table, string column) GetFullyQualifiedColumnNameParts(this string value)
    {
        Debug.Assert(IsFullyQualifiedColumnName(value));

        var openIndex = value.IndexOf('[');
        if (openIndex == 0) throw new InvalidOperationException("Invalid open parenthesis index");

        var closeIndex = value.LastIndexOf(']');
        if (closeIndex != value.Length - 1) throw new InvalidOperationException("Invalid close parenthesis index");

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

        throw new InvalidOperationException($"Unexpected token type {tokenType}");
    }
}
