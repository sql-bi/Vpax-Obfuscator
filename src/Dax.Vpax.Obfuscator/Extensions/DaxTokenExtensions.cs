using System.Diagnostics;
using Dax.Tokenizer;

namespace Dax.Vpax.Obfuscator.Extensions;

internal static class DaxTokenExtensions
{
    public static bool IsFullyQualifiedColumnName(this DaxToken token)
    {
        Debug.Assert(token.Type == DaxToken.STRING_LITERAL|| token.Type == DaxToken.COLUMN_OR_MEASURE);
        return token.Text.EndsWith("]") && token.Text.IndexOf('[') > 0;
    }

    public static bool IsVariable(this DaxToken token)
    {
        Debug.Assert(token.Type == DaxToken.TABLE_OR_VARIABLE);
        return token.Type == DaxToken.TABLE_OR_VARIABLE && !IsFunction(token);
    }

    public static bool IsFunction(this DaxToken token)
    {
        Debug.Assert(token.Type == DaxToken.TABLE_OR_VARIABLE);

        var current = token.Next;
        while (current != null && current.CommentOrWhitespace)
            current = current.Next;

        return current != null && current.Type == DaxToken.OPEN_PARENS;
    }

    public static bool IsReservedExtensionColumn(this DaxToken token)
    {
        Debug.Assert(token.Type == DaxToken.COLUMN_OR_MEASURE);

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

    public static (string table, string column) GetFullyQualifiedColumnNameParts(this DaxToken token)
    {
        Debug.Assert(token.IsFullyQualifiedColumnName());

        var openIndex = token.Text.IndexOf('[');
        if (openIndex == 0) throw new InvalidOperationException("Invalid open parenthesis index");

        var closeIndex = token.Text.LastIndexOf(']');
        if (closeIndex != token.Text.Length - 1) throw new InvalidOperationException("Invalid close parenthesis index");

        var table = token.Text.Substring(0, openIndex);
        var column = token.Text.Substring(openIndex + 1, closeIndex - openIndex - 1);
        return (table, column);
    }

    public static string Replace(this DaxToken token, string expression, DaxText text)
        => Replace(token, expression, text.ObfuscatedValue);

    public static string Replace(this DaxToken token, string expression, string value)
    {
        var substring = expression.Substring(token.StartIndex, token.StopIndex - token.StartIndex + 1);

        switch (token.Type)
        {
            case DaxToken.TABLE:
            case DaxToken.STRING_LITERAL:
            case DaxToken.COLUMN_OR_MEASURE:
                return string.Concat(substring[0], value, substring[substring.Length - 1]);
            case DaxToken.UNTERMINATED_TABLEREF:
            case DaxToken.UNTERMINATED_COLREF:
                return string.Concat(substring[0], value);
        }

        return substring.Replace(token.Text, value);
    }
}
