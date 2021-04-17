using System;
using System.Collections.Generic;
using System.Text;

namespace Interpretator
{
    /*------------------------------------------------------------------
     * PARSER RULES
     *------------------------------------------------------------------*/

    //    expr : plusminus* EOF ;
    //
    //    plusminus: multdiv ( ( '+' | '-' ) multdiv )* ;
    //
    //    multdiv : factor ( ( '*' | '/' ) factor )* ;
    //
    //    factor : NUMBER | '(' expr ')' ;

    public class Interpretator
    {
        public List<Lexeme> LexAnalyze(string expText)
        {
            List<Lexeme> lexemes = new List<Lexeme>();
            int pos = 0;

            while (pos < expText.Length)
            {
                char c = expText.ToCharArray()[pos];

                if (c == '(')
                {
                    lexemes.Add(new Lexeme(LexemeType.LEFT_BRACKET, c));
                    pos++;
                    continue;
                }
                else if (c == ')')
                {
                    lexemes.Add(new Lexeme(LexemeType.RIGHT_BRACKET, c));
                    pos++;
                    continue;
                }
                else if (c == '+')
                {
                    lexemes.Add(new Lexeme(LexemeType.OP_PLUS, c));
                    pos++;
                    continue;
                }
                else if (c == '-')
                {
                    lexemes.Add(new Lexeme(LexemeType.OP_MINUS, c));
                    pos++;
                    continue;
                }
                else if (c == '*')
                {
                    lexemes.Add(new Lexeme(LexemeType.OP_MUL, c));
                    pos++;
                    continue;
                }
                else if (c == '/')
                {
                    lexemes.Add(new Lexeme(LexemeType.OP_DIV, c));
                    pos++;
                    continue;
                }
                else
                {
                    if (c <= '9' && c >= '0')
                    {
                        StringBuilder sb = new StringBuilder();

                        do
                        {
                            sb.Append(c);
                            pos++;

                            if (pos >= expText.Length)
                            {
                                break;
                            }

                            c = expText.ToCharArray()[pos];

                        } while (c <= '9' && c >= '0');

                        lexemes.Add(new Lexeme(LexemeType.NUMBER, sb.ToString()));
                    }
                    else
                    {
                        if (c != ' ')
                        {
                            throw new Exception("Unexpected character: " + c);
                        }

                        pos++;
                    }
                }
            }

            lexemes.Add(new Lexeme(LexemeType.EOF, string.Empty));

            return lexemes;
        }

        public int Expr(LexemeBuffer lexemes)
        {
            Lexeme lexeme = lexemes.Next();

            if (lexeme.Type == LexemeType.EOF)
            {
                return 0;
            }
            else
            {
                lexemes.Back();
                return Plusminus(lexemes);
            }
        }

        private int Plusminus(LexemeBuffer lexemes)
        {
            int value = Multdiv(lexemes);

            while (true)
            {
                Lexeme lexeme = lexemes.Next();

                switch (lexeme.Type)
                {
                    case LexemeType.OP_PLUS:
                        value += Multdiv(lexemes);
                        break;
                    case LexemeType.OP_MINUS:
                        value -= Multdiv(lexemes);
                        break;
                    case LexemeType.EOF:
                    case LexemeType.RIGHT_BRACKET:
                        lexemes.Back();
                        return value;
                    default:
                        throw new Exception("Unexpected token: " + lexeme.Value + " at position: " + lexemes.GetPos());
                }
            }
        }

        private int Multdiv(LexemeBuffer lexemes)
        {
            int value = Factor(lexemes);

            while (true)
            {
                Lexeme lexeme = lexemes.Next();

                switch (lexeme.Type)
                {
                    case LexemeType.OP_MUL:
                        value *= Factor(lexemes);
                        break;
                    case LexemeType.OP_DIV:
                        value /= Factor(lexemes);
                        break;
                    case LexemeType.EOF:
                    case LexemeType.RIGHT_BRACKET:
                    case LexemeType.OP_PLUS:
                    case LexemeType.OP_MINUS:
                        lexemes.Back();
                        return value;
                    default:
                        throw new Exception("Unexpected token: " + lexeme.Value + " at position: " + lexemes.GetPos());
                }
            }
        }

        private int Factor(LexemeBuffer lexemes)
        {
            Lexeme lexeme = lexemes.Next();

            switch (lexeme.Type)
            {
                case LexemeType.NUMBER:
                    return int.Parse(lexeme.Value);
                case LexemeType.LEFT_BRACKET:
                    int value = Plusminus(lexemes);
                    lexeme = lexemes.Next();

                    if (lexeme.Type != LexemeType.RIGHT_BRACKET)
                    {
                        throw new Exception("Unexpected token: " + lexeme.Value + " at position: " + lexemes.GetPos());
                    }

                    return value;
                default:
                    throw new Exception("Unexpected token: " + lexeme.Value + " at position: " + lexemes.GetPos());
            }
        }
    }
}