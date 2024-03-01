using Dax.Vpax.Obfuscator.Extensions;
using System.Text;
using Dax.Tokenizer;

namespace Dax.Vpax.Obfuscator;

internal sealed partial class DaxModelDeobfuscator
{
    internal string DeobfuscateExpression(string expression)
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
                    tokenText = _dictionary.GetValue(tokenText);
                    break;
                case DaxToken.COLUMN_OR_MEASURE when token.IsReservedExtensionColumn():
                    tokenText = token.Replace(expression, tokenText);
                    break;
                case DaxToken.COLUMN_OR_MEASURE when token.IsFullyQualifiedColumnName():
                case DaxToken.STRING_LITERAL when token.IsFullyQualifiedColumnName():
                    tokenText = ReplaceExtensionColumnName(token);
                    break;
                case DaxToken.TABLE_OR_VARIABLE when token.IsVariable():
                case DaxToken.TABLE:
                case DaxToken.COLUMN_OR_MEASURE:
                case DaxToken.STRING_LITERAL:
                case DaxToken.UNTERMINATED_COLREF:
                case DaxToken.UNTERMINATED_TABLEREF:
                case DaxToken.UNTERMINATED_STRING:
                    tokenText = token.Replace(expression, _dictionary.GetValue(tokenText));
                    break;
            }

            builder.Append(tokenText);
        }

        return builder.ToString();

        string ReplaceExtensionColumnName(DaxToken token)
        {
            var (tableName, columnName) = token.GetFullyQualifiedColumnNameParts();
            var table = _dictionary.GetValue(tableName);
            var column = _dictionary.GetValue(columnName);

            var escape = token.Type == DaxToken.STRING_LITERAL;
            if (escape)
            {
                table = table.DaxEscape();
                column = column.DaxEscape();
            }

            var value = $"{table}[{column}]";
            return token.Replace(expression, value, escape);
        }
    }
}
