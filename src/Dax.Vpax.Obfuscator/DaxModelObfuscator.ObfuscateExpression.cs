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
                case DaxToken.COLUMN_OR_MEASURE when token.IsReservedTokenOrKeyword():
                    tokenText = token.Replace(expression, tokenText);
                    break;
                case DaxToken.TABLE_OR_VARIABLE when token.IsVariable():
                case DaxToken.TABLE:
                case DaxToken.UNTERMINATED_TABLEREF:
                case DaxToken.COLUMN_OR_MEASURE:
                case DaxToken.UNTERMINATED_COLREF:
                case DaxToken.STRING_LITERAL:
                case DaxToken.UNTERMINATED_STRING:
                    {
                        if (token.IsString() && token.Text.TryGetTableAndColumnNames(out var table, out var column))
                        {
                            var value = ObfuscateTableAndColumnNames(table, column, token);
                            tokenText = token.Replace(expression, value);
                        }
                        else
                        {
                            if (token.IsColumnOrMeasure()) tokenText = tokenText.UnescapeDax(DaxToken.COLUMN_OR_MEASURE);
                            if (token.IsString()) tokenText = tokenText.UnescapeDax(DaxToken.STRING_LITERAL);
                            if (token.IsTable()) tokenText = tokenText.UnescapeDax(DaxToken.TABLE);
                            var value = ObfuscateText(new DaxText(tokenText)).ObfuscatedValue;
                            tokenText = token.Replace(expression, value);
                        }
                    }
                    break;
            }

            builder.Append(tokenText);
        }

        return builder.ToString();
    }

    internal string ObfuscateTableAndColumnNames(string table, string column, DaxToken? token = null)
    {
        table = table.UnescapeDax(DaxToken.TABLE).Trim();
        column = column.UnescapeDax(DaxToken.COLUMN_OR_MEASURE);

        var quoted = table.IsQuoted();
        if (quoted) table = table.Unquote();

        var tableName = ObfuscateText(new DaxText(table)).ObfuscatedValue;
        var columnName = ObfuscateText(new DaxText(column)).ObfuscatedValue;

        if (quoted) tableName = $"'{tableName}'";
        return $"{tableName}[{columnName}]";
    }
}
