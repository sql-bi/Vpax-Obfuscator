using System.Diagnostics;
using Dax.Tokenizer;

namespace Dax.Vpax.Obfuscator.Extensions;

internal static class DaxTokenExtensions
{
    public static bool IsFullyQualifiedColumnName(this DaxToken token)
        => token.Text.EndsWith("]") && token.Text.IndexOf('[') > 0;

    public static bool IsVariable(this DaxToken token)
        => token.Type == DaxToken.TABLE_OR_VARIABLE && !IsFunction(token);

    public static bool IsFunction(this DaxToken token)
    {
        if (token.Type != DaxToken.TABLE_OR_VARIABLE) return false;

        var current = token.Next;
        while (current != null && current.CommentOrWhitespace)
            current = current.Next;

        return current != null && current.Type == DaxToken.OPEN_PARENS;
    }

    public static bool IsReservedExtensionColumn(this DaxToken token)
    {
        if (token.Type != DaxToken.COLUMN_OR_MEASURE) return false;

        if (token.Text.StartsWith(DaxTextObfuscator.ReservedToken_Value, StringComparison.OrdinalIgnoreCase))
        {
            // ''[Value] extension column OR table constructor { } extension column when there is only one column
            if (token.Text.Length == DaxTextObfuscator.ReservedToken_Value.Length)
                return true;

            // Table constructor { } extension column when there are N columns
            if (int.TryParse(token.Text.Substring(DaxTextObfuscator.ReservedToken_Value.Length), out _))
                return true;
        }
        else if (token.Text.Equals(DaxTextObfuscator.ReservedToken_Date, StringComparison.OrdinalIgnoreCase))
        {
            return true; // CALENDAR() [Date] extension column
        }

        return false;
    }

    public static (string tableName, string columnName) GetFullyQualifiedColumnNameParts(this DaxToken token)
    {
        Debug.Assert(token.IsFullyQualifiedColumnName());

        var openIndex = token.Text.IndexOf('[');
        var closeIndex = token.Text.LastIndexOf(']');
        var tableName = token.Text.Substring(0, openIndex);
        var columnName = token.Text.Substring(openIndex + 1, closeIndex - openIndex - 1);
        return (tableName, columnName);
    }

    public static string Replace(this DaxToken token, string expression, DaxText text)
        => Replace(token, expression, text.ObfuscatedValue);

    public static string Replace(this DaxToken token, string expression, string value, bool escape = false)
    {
        var substring = expression.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);
        var tokenText = escape ? token.Text.DaxEscape() : token.Text;

        if (substring.IndexOf(tokenText, StringComparison.Ordinal) == -1)
            throw new InvalidOperationException($"Failed to replace token >> {token.Type} | {substring} | {tokenText} | {value}");

        return substring.Replace(tokenText, value);
    }
}
