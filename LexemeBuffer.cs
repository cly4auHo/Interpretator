using System.Collections.Generic;

namespace Interpretator
{
    public class LexemeBuffer
    {
        private readonly List<Lexeme> lexemes;
        private int position;

        public Lexeme Next => lexemes[position++];
        public int Position => position;

        public void Back() => position--;
        
        public LexemeBuffer(List<Lexeme> lexemes)
        {
            this.lexemes = lexemes;
        }
    }
}