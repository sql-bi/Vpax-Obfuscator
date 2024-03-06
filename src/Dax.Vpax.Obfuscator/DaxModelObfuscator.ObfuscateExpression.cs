using System.Text;
using Dax.Tokenizer;
using Dax.Vpax.Obfuscator.Extensions;

namespace Dax.Vpax.Obfuscator;

internal sealed partial class DaxModelObfuscator
{
    internal string ObfuscateExpression(string expression)
    {
        var tokens = DaxTokenizer.Tokenize(expression, DaxLocale.US, includeHidden: true);
        var builder = new StringBuilder(expression.Length);

        foreach (var token in tokens)
        {
            var tokenText = token.Text;

            if (tokenText.Length == 0)
            {
                switch (token.Type)
                {
                    case DaxToken.STRING_LITERAL:
                        builder.Append("\"\"");
                        break;
                    case DaxToken.TABLE:
                        builder.Append("''");
                        break;
                }
                continue;
            }

            switch (token.Type)
            {
                case DaxToken.SINGLE_LINE_COMMENT:
                case DaxToken.DELIMITED_COMMENT:
                    tokenText = ObfuscateText(new DaxText(tokenText)).ObfuscatedValue;
                    break;
                case DaxToken.COLUMN_OR_MEASURE when token.IsReservedTokenName():
                    tokenText = token.Replace(expression, tokenText);
                    break;
                case DaxToken.TABLE_OR_VARIABLE when token.IsVariable():
                case DaxToken.TABLE:
                case DaxToken.COLUMN_OR_MEASURE:
                case DaxToken.STRING_LITERAL:
                case DaxToken.UNTERMINATED_COLREF:
                case DaxToken.UNTERMINATED_TABLEREF:
                case DaxToken.UNTERMINATED_STRING:
                    {
                        if (token.Text.IsFullyQualifiedColumnName())
                        {
                            var value = ObfuscateFullyQualifiedColumnName(tokenText).EscapeDax(token.Type);
                            tokenText = token.Replace(expression, value);
                        }
                        else
                        {
                            var value = ObfuscateText(new DaxText(tokenText)).ObfuscatedValue.EscapeDax(token.Type);
                            tokenText = token.Replace(expression, value);
                        }
                    }
                    break;
            }

            builder.Append(tokenText);
        }

        return builder.ToString();
    }

    internal string ObfuscateFullyQualifiedColumnName(string value)
    {
        var (table, column) = value.GetFullyQualifiedColumnNameParts();

        var tableText = new DaxText(table);
        var columnText = new DaxText(column);

        if (IsSquareBraketsRequired(table, column))
        {
            // Since the plaintext value contains at least one character that results in a fully qualified
            // column name that requires square brackets, then, in order to preserve the same semantics
            // we must add at least a single char of the same type to the obfuscated value as well.
            // We use the reserved char '-' for this purpose becase it is preserved by the obfuscator.
            tableText = new DaxText($"{DaxTextObfuscator.ReservedChar_Minus}{table}");
            tableText.PlaintextValue = table;
        }

        var tableName = ObfuscateText(tableText).ObfuscatedValue;
        var columnName = ObfuscateText(columnText).ObfuscatedValue;

        return $"{tableName}[{columnName}]";

        static bool IsSquareBraketsRequired(string table, string column)
        {
            if (table.Length > 0)
            {
                table = table.Trim(); // remove any leading or trailing whitespace

                if ("0123456789".Contains(table[0]))
                    return true; // Table name start with a digit

                if (table.Any((c) => c != '_' && !DaxTextObfuscator.CharSet.Contains(c)))
                    return true; // Table name contains any non-alphabetic characters except for the underscore
            }

            return column.Contains(']');
        }
    }
}
