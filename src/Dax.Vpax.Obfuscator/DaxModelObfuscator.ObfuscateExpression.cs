using System.Text;
using Dax.Tokenizer;
using Dax.Vpax.Obfuscator.Extensions;

namespace Dax.Vpax.Obfuscator;

internal sealed partial class DaxModelObfuscator
{
    internal string ObfuscateExpression(string expression)
    {
        // TODO: refactor to deduplicate the code with DaxModelDeobfuscator.DeobfuscateExpression

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
                case DaxToken.TABLE_OR_VARIABLE when token.IsFunction() && _identifiers.IsKnownUserDefinedFunction(tokenText):
                case DaxToken.OTHER_IDENTIFIER when token.IsFunction() && _identifiers.IsKnownUserDefinedFunction(tokenText):
                    {
                        tokenText = ObfuscateText(new DaxText(tokenText), ObfuscationRule.None);
                    }
                    break;
                case DaxToken.TABLE:
                case DaxToken.TABLE_OR_VARIABLE when token.IsVariable():
                case DaxToken.UNTERMINATED_TABLEREF:
                    {
                        var value = ObfuscateText(new DaxText(tokenText), ObfuscationRule.PreserveDaxKeywords);
                        tokenText = token.Replace(expression, value);
                    }
                    break;
                case DaxToken.COLUMN_OR_MEASURE:
                case DaxToken.UNTERMINATED_COLREF:
                    {
                        var value = ObfuscateText(new DaxText(tokenText), ObfuscationRule.PreserveDaxReservedNames);
                        tokenText = token.Replace(expression, value);
                    }
                    break;
                case DaxToken.STRING_LITERAL:
                case DaxToken.UNTERMINATED_STRING:
                    {
                        if (token.Text.TryGetTableAndColumnNames(out var table, out var column))
                        {
                            var value = ObfuscateTableAndColumnNames(table, column);
                            tokenText = token.Replace(expression, value);
                        }
                        else
                        {
                            var value = ObfuscateText(new DaxText(tokenText));
                            tokenText = token.Replace(expression, value);
                        }
                    }
                    break;
            }

            builder.Append(tokenText);
        }

        return builder.ToString();
    }

    internal string ObfuscateTableAndColumnNames(string table, string column)
    {
        table = table.UnescapeDax(DaxToken.TABLE).Trim();
        column = column.UnescapeDax(DaxToken.COLUMN_OR_MEASURE);

        var quoted = table.IsQuoted();
        if (quoted) table = table.Unquote();

        var tableName = ObfuscateText(new DaxText(table));
        var columnName = ObfuscateText(new DaxText(column));

        if (quoted) tableName = $"'{tableName}'";
        return $"{tableName}[{columnName}]";
    }
}
