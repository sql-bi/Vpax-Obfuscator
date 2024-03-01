using Dax.Vpax.Obfuscator.Extensions;
using System.Text;
using Dax.Tokenizer;

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
                case DaxToken.COLUMN_OR_MEASURE when token.IsReservedExtensionColumn():
                    tokenText = token.Replace(expression, tokenText);
                    break;
                case DaxToken.STRING_LITERAL when token.IsExtensionColumnName():
                    tokenText = ReplaceExtensionColumnName(token);
                    break;
                case DaxToken.TABLE_OR_VARIABLE when token.IsVariable():
                case DaxToken.TABLE:
                case DaxToken.COLUMN_OR_MEASURE:
                case DaxToken.STRING_LITERAL:
                case DaxToken.UNTERMINATED_COLREF:
                case DaxToken.UNTERMINATED_TABLEREF:
                case DaxToken.UNTERMINATED_STRING:
                    tokenText = token.Replace(expression, ObfuscateText(new DaxText(tokenText)));
                    break;
            }

            builder.Append(tokenText);
        }

        return builder.ToString();

        string ReplaceExtensionColumnName(DaxToken token)
        {
            var (tableName, columnName) = token.GetExtensionColumnNameParts();
            var tableText = ObfuscateText(new DaxText(tableName));
            var columnText = ObfuscateText(new DaxText(columnName));

            var value = $"{tableText.ObfuscatedValue.DaxEscape()}[{columnText.ObfuscatedValue.DaxEscape()}]";
            return token.Replace(expression, value, escape: true);
        }
    }
}
