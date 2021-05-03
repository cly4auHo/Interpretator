using System;
using System.Collections.Generic;

namespace Interpretator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string expressionText = "avg(3, 4, 2 * 3 + pow(2,3)) - min(-1, 3)";

            Interpretator interpretator = new Interpretator();
            List<Lexeme> lexemes = interpretator.LexAnalyze(expressionText);
            LexemeBuffer lexemeBuffer = new LexemeBuffer(lexemes);  
            
            Console.WriteLine(interpretator.Expr(lexemeBuffer)); //output: 8
        }     
    }
}