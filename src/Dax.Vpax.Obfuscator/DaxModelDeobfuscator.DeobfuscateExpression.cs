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
                    tokenText = DeobfuscateText(tokenText);
                    break;
                case DaxToken.TABLE:
                case DaxToken.TABLE_OR_VARIABLE when token.IsVariable():
                case DaxToken.UNTERMINATED_TABLEREF:
                    {
                        var value = DeobfuscateText(tokenText, ObfuscationRule.PreserveDaxKeywords).EscapeDax(token.Type);
                        tokenText = token.Replace(expression, value);
                    }
                    break;
                case DaxToken.COLUMN_OR_MEASURE:
                case DaxToken.UNTERMINATED_COLREF:
                    {
                        var value = DeobfuscateText(tokenText, ObfuscationRule.PreserveDaxReservedNames).EscapeDax(token.Type);
                        tokenText = token.Replace(expression, value);
                    }
                    break;
                case DaxToken.STRING_LITERAL:
                case DaxToken.UNTERMINATED_STRING:
                    {
                        if (token.Text.TryGetTableAndColumnNames(out var table, out var column))
                        {
                            var value = DeobfuscateTableAndColumnNames(table, column).EscapeDax(token.Type);
                            tokenText = token.Replace(expression, value);
                        }
                        else
                        {
                            var value = DeobfuscateText(tokenText).EscapeDax(token.Type);
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

        var tableName = DeobfuscateText(table).EscapeDax(DaxToken.TABLE);
        var columnName = DeobfuscateText(column).EscapeDax(DaxToken.COLUMN_OR_MEASURE);

        if (quoted) tableName = $"'{tableName}'";
        return $"{tableName}[{columnName}]";
    }
}
