using Antlr4.Runtime;

namespace Dax.Tokenizer
{
    public sealed partial class DaxToken
    {
        public int Type { get; private set; }
        public int TokenIndex { get; private set; }
        public int StartIndex { get; private set; }
        public int StopIndex { get; private set; }
        public string Text { get; private set; }

        private readonly IList<DaxToken> Collection;

        /// <summary>
        /// Get the next token
        /// </summary>
        public DaxToken Prev
        {
            get
            {
                var ix = TokenIndex - 1;
                while (ix >= 0)
                {
                    var token = Collection[ix];
                    if (!token.CommentOrWhitespace) return token;
                    ix--;
                }
                return null;
            }
        }

        public DaxToken Next
        {
            get
            {
                var ix = TokenIndex + 1;
                while (ix < Collection.Count)
                {
                    var token = Collection[ix];
                    if (!token.CommentOrWhitespace) return token;
                    ix++;
                }
                return null;
            }
        }

        public bool CommentOrWhitespace { get; private set; }

        internal DaxToken(IToken antlrToken, IList<DaxToken> collection)
        {
            Type = antlrToken.Type;
            TokenIndex = antlrToken.TokenIndex;
            StartIndex = antlrToken.StartIndex;
            StopIndex = antlrToken.StopIndex;
            Text = antlrToken.Text;
            CommentOrWhitespace = antlrToken.Channel == DaxLexer.COMMENTS_CHANNEL || antlrToken.Channel == DaxLexer.Hidden;
            Collection = collection;
        }

        public override string ToString()
        {
            var cow = CommentOrWhitespace ? "*** " : "";
            return $"Token #{TokenIndex}, {cow}{StartIndex}-{StopIndex}: DaxToken.{DaxLexer.DefaultVocabulary.GetSymbolicName(Type)}: \"{Text}\"";
        }
    }
}
