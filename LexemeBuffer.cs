using System.Collections.Generic;

namespace Interpretator
{
    public class LexemeBuffer
    {
        private readonly List<Lexeme> lexemes;
        private int pos;

        public LexemeBuffer(List<Lexeme> lexemes)
        {
            this.lexemes = lexemes;
        }

        public Lexeme Next()
        {
            return lexemes[pos++];
        }

        public void Back()
        {
            pos--;
        }

        public int GetPos()
        {
            return pos;
        }
    }
}