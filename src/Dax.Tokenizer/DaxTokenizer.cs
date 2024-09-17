using Antlr4.Runtime;

[assembly: CLSCompliant(false)]
namespace Dax.Tokenizer
{
    public static class DaxTokenizer
    {
        public static IReadOnlyList<DaxToken> Tokenize(string dax, DaxLocale locale = DaxLocale.US, bool includeHidden = true)
        {
            var tokens = TokenizeImpl(dax, locale, includeHidden);
            var result = new List<DaxToken>();
            result.AddRange(tokens.Select(t => new DaxToken(t, result)));
            return result;
        }

        private static List<IToken> TokenizeImpl(string dax, DaxLocale locale, bool includeHidden)
        {
            dax ??= string.Empty;
            var input = new DaxCharStream(dax);

            var lexer = new DaxLexer(input, locale);
            var tokenStream = new BufferedTokenStream(lexer);
            lexer.RemoveErrorListeners();
            var tokens = new List<IToken>(includeHidden ? Math.Min(dax.Length / 3, 32) : Math.Min(dax.Length / 4, 16));
            if (includeHidden)
            {
                var t = lexer.NextToken();
                int i = 0;
                while (t.Type != TokenConstants.EOF)
                {
                    var wt = (IWritableToken)t;
                    wt.TokenIndex = i;
                    wt.Line = t.Line - 1; // Shift ANTLRs line numbering (1..n) -> (0..n)
                    tokens.Add(t);
                    i++;
                    t = lexer.NextToken();
                }
            }
            else
            {
                var t = lexer.NextToken();
                int i = 0;
                while (t.Type != TokenConstants.EOF)
                {
                    if (t.Channel == DaxLexer.DefaultTokenChannel)
                    {
                        var wt = (IWritableToken)t;
                        wt.TokenIndex = i;
                        wt.Line = t.Line - 1; // Shift ANTLRs line numbering (1..n) -> (0..n)
                        tokens.Add(t);
                        i++;
                    }
                    t = lexer.NextToken();
                }
            }
            return tokens;
        }
    }
}
