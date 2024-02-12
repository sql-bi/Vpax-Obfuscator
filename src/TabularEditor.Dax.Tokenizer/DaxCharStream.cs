using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace TabularEditor.Dax.Tokenizer
{
    public class DaxCharStream : ICharStream
    {
        private ICharStream stream;

        public DaxCharStream(string dax) : this(new AntlrInputStream(dax))
        {
        }

        public DaxCharStream(ICharStream stream)
        {
            this.stream = stream;
        }

        public int Index
        {
            get
            {
                return stream.Index;
            }
        }

        public int Size {
            get {
                return stream.Size;
            }
        }

        public string SourceName
        {
            get
            {
                return stream.SourceName;
            }
        }

        public void Consume()
        {
            stream.Consume();
        }

        [return: NotNull]
        public string GetText(Interval interval)
        {
            return stream.GetText(interval);
        }

        public int LA(int i)
        {
            int c = stream.LA(i);

            if (c <= 0)
            {
                return c;
            }

            char o = (char)c;

            return (int)char.ToUpperInvariant(o);
        }

        public int Mark()
        {
            return stream.Mark();
        }

        public void Release(int marker)
        {
            stream.Release(marker);
        }

        public void Seek(int index)
        {
            stream.Seek(index);
        }
    }
}