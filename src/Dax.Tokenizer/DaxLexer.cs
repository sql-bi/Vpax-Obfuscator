using Antlr4.Runtime;

namespace TabularEditor.Dax.Tokenizer
{
    internal partial class DaxLexer
    {
        public DaxLexer(ICharStream input, DaxLocale locale) : this(input)
        {
            this.NonUsLocale = locale == DaxLocale.NonUS;
            this.UsLocale = locale == DaxLocale.US;
        }

        private readonly bool NonUsLocale;
        private readonly bool UsLocale;
    }
}
