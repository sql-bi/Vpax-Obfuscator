using System.Diagnostics;
using Dax.Tokenizer;

namespace Dax.Vpax.Obfuscator.Extensions;

internal static class StringExtensions
{
    public static bool IsFullyQualifiedColumnName(this string value)
        => value.TrimEnd().EndsWith("]") && value.IndexOf('[') > 0;

    public static (string table, string column) GetFullyQualifiedColumnNameParts(this string value, bool obfuscating = false)
    {
        Debug.Assert(IsFullyQualifiedColumnName(value));

        var openIndex = value.IndexOf('[');
        var closeIndex = value.LastIndexOf(']');
        var table = value.Substring(0, openIndex);
        var column = value.Substring(openIndex + 1, closeIndex - openIndex - 1);

        if (obfuscating)
        {
            table = table.Trim(); // remove any leading or trailing whitespace first

            if (IsSquareBraketsRequired(table, column))
            {
                // Since the plaintext value contains at least one character that results in a fully qualified
                // column name that requires square brackets, then, in order to preserve the same semantics
                // we must add at least a single char of the same type to the obfuscated value as well.
                table = $"{DaxTextObfuscator.ReservedChar_Minus}{table}";
            }
        }

        return (table, column);

        static bool IsSquareBraketsRequired(string table, string column)
        {
            if (table.Length > 0)
            {
                if ("012345679".Contains(table[0]))
                    return true; // Table name start with a digit

                if (table.Any((c) => c != '_' && !DaxTextObfuscator.CharSet.Contains(c)))
                    return true; // Table name contains any non-alphabetic characters except for the underscore
            }

            return column.Contains(']');
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
}
