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
                case DaxToken.TABLE_OR_VARIABLE when token.IsVariable():
                case DaxToken.TABLE:
                case DaxToken.UNTERMINATED_TABLEREF:
                case DaxToken.COLUMN_OR_MEASURE:
                case DaxToken.UNTERMINATED_COLREF:
                case DaxToken.STRING_LITERAL:
                case DaxToken.UNTERMINATED_STRING:
                    {
                        if (token.IsStringLiteral() && token.Text.TryGetTableAndColumnNames(out var table, out var column))
                        {
                            var value = DeobfuscateTableAndColumnNames(table, column);
                            tokenText = token.Replace(expression, value);
                        }
                        else
                        {
                            var value = _dictionary.GetValue(tokenText);
                            if (token.IsColumnOrMeasure()) value = value.EscapeDax(DaxToken.COLUMN_OR_MEASURE);
                            if (token.IsStringLiteral()) value = value.EscapeDax(DaxToken.STRING_LITERAL);
                            if (token.IsTable()) value = value.EscapeDax(DaxToken.TABLE);
                            tokenText = token.Replace(expression, value);
                        }
                    }
                    break;
            }

            builder.Append(tokenText);
        }

        return builder.ToString();
    }

    internal string DeobfuscateTableAndColumnNames(string table, string column)
    {
        var quoted = table.IsQuoted();
        if (quoted) table = table.Unquote();

        var tableName = _dictionary.GetValue(table);
        var columnName = _dictionary.GetValue(column);

        tableName = tableName.EscapeDax(DaxToken.TABLE);
        columnName = columnName.EscapeDax(DaxToken.COLUMN_OR_MEASURE);

        if (quoted) tableName = $"'{tableName}'";
        return $"{tableName}[{columnName}]".EscapeDax(DaxToken.STRING_LITERAL);
    }
}
