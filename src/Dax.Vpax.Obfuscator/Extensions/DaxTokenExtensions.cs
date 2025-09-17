using Dax.Tokenizer;

namespace Dax.Vpax.Obfuscator.Extensions;

internal static class DaxTokenExtensions
{
    public static bool IsVariable(this DaxToken token) => !IsFunction(token);

    /// <summary>
    /// Determines whether the specified token represents a DAX function call.
    /// </summary>
    /// <remarks>
    /// This applies to both built-in and user-defined functions.
    /// </remarks>
    public static bool IsFunction(this DaxToken token)
    {
        if (token.Type is not (DaxToken.TABLE_OR_VARIABLE or DaxToken.OTHER_IDENTIFIER))
            throw new InvalidOperationException($"Invalid token for function detection. Token value: '{token.Text}', type: {token.Type}.");

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
