using System.Diagnostics;
using Dax.Tokenizer;

namespace Dax.Vpax.Obfuscator.Extensions;

internal static class DaxTokenExtensions
{
    public static bool IsStringLiteral(this DaxToken? token) => token?.Type == DaxToken.STRING_LITERAL || token?.Type == DaxToken.UNTERMINATED_STRING;
    public static bool IsTable(this DaxToken? token) => token?.Type == DaxToken.TABLE || (token?.Type == DaxToken.TABLE_OR_VARIABLE && !IsVariable(token));
    public static bool IsColumnOrMeasure(this DaxToken? token) => token?.Type == DaxToken.COLUMN_OR_MEASURE || token?.Type == DaxToken.UNTERMINATED_COLREF;

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
