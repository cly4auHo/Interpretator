using System;
using System.Collections.Generic;

namespace Interpretator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string expressionText = "10 - 4 * (2 + 2/8) -1 ";

            Interpretator interpretator = new Interpretator();
            List<Lexeme> lexemes = interpretator.LexAnalyze(expressionText);
            LexemeBuffer lexemeBuffer = new LexemeBuffer(lexemes);

            Console.WriteLine(interpretator.Expr(lexemeBuffer));
        }     
    }
}