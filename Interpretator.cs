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
    //    factor : func | unary | NUMBER | '(' expr ')' ;
    //
    //    unary : '-' factor
    //
    //    func : NAME '(' (expr (',' expr)+)? ')'

    public class Interpretator
    {
        private readonly IReadOnlyDictionary<string, IFunction> functionMap;

        public List<Lexeme> LexAnalyze(string expText)
        {
            List<Lexeme> lexemes = new List<Lexeme>();
            int pos = 0;

            while (pos < expText.Length)
            {
                char c = expText[pos];

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
                else if (c == ',')
                {
                    lexemes.Add(new Lexeme(LexemeType.COMMA, c));
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
                                break;

                            c = expText[pos];

                        } while (c <= '9' && c >= '0');

                        lexemes.Add(new Lexeme(LexemeType.NUMBER, sb.ToString()));
                    }
                    else
                    {
                        if (c != ' ')
                        {
                            if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
                            {
                                StringBuilder sb = new StringBuilder();

                                do
                                {
                                    sb.Append(c.ToString().ToUpper());
                                    pos++;

                                    if (pos >= expText.Length)
                                        break;

                                    c = expText[pos];

                                } while (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z');

                                if (functionMap.ContainsKey(sb.ToString()))                              
                                    lexemes.Add(new Lexeme(LexemeType.NAME, sb.ToString()));                              
                                else                          
                                    throw new Exception("Unexpected character: " + c);                              
                            }
                        }
                        else
                        {
                            pos++;
                        }
                    }
                }
            }

            lexemes.Add(new Lexeme(LexemeType.EOF, string.Empty));

            return lexemes;
        }

        public int Expr(LexemeBuffer lexemes)
        {
            Lexeme lexeme = lexemes.Next;

            if (lexeme.Type == LexemeType.EOF)          
                return 0;           
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
                Lexeme lexeme = lexemes.Next;

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
                    case LexemeType.COMMA:
                        lexemes.Back();
                        return value;
                    default:
                        throw new Exception("Unexpected token: " + lexeme.Value + " at position: " + lexemes.Position);
                }
            }
        }

        private int Multdiv(LexemeBuffer lexemes)
        {
            int value = Factor(lexemes);

            while (true)
            {
                Lexeme lexeme = lexemes.Next;

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
                    case LexemeType.COMMA:
                        lexemes.Back();
                        return value;
                    default:
                        throw new Exception("Unexpected token: " + lexeme.Value + " at position: " + lexemes.Position);
                }
            }
        }

        private int Factor(LexemeBuffer lexemes)
        {
            Lexeme lexeme = lexemes.Next;

            switch (lexeme.Type)
            {
                case LexemeType.NUMBER:
                    return int.Parse(lexeme.Value);
                case LexemeType.NAME:
                    lexemes.Back();
                    return Func(lexemes);
                case LexemeType.OP_MINUS:
                    int value = Factor(lexemes);
                    return -value;
                case LexemeType.LEFT_BRACKET:
                    value = Plusminus(lexemes);
                    lexeme = lexemes.Next;

                    if (lexeme.Type != LexemeType.RIGHT_BRACKET)                  
                        throw new Exception("Unexpected token: " + lexeme.Value + " at position: " + lexemes.Position);                   

                    return value;
                default:
                    throw new Exception("Unexpected token: " + lexeme.Value + " at position: " + lexemes.Position);
            }
        }

        private int Func(LexemeBuffer lexemeBuffer)
        {
            string name = lexemeBuffer.Next.Value;
            Lexeme lexeme = lexemeBuffer.Next;

            if (lexeme.Type != LexemeType.LEFT_BRACKET)          
                throw new Exception("Wrong function call syntax at " + lexeme.Value);
           
            List<int> args = new List<int>();
            lexeme = lexemeBuffer.Next;

            if (lexeme.Type != LexemeType.RIGHT_BRACKET)
            {
                lexemeBuffer.Back();

                do
                {
                    args.Add(Expr(lexemeBuffer));
                    lexeme = lexemeBuffer.Next;

                    if (lexeme.Type != LexemeType.COMMA && lexeme.Type != LexemeType.RIGHT_BRACKET)                  
                        throw new Exception("Wrong function call syntax at " + lexeme.Value);                 
                } while (lexeme.Type == LexemeType.COMMA);
            }

            functionMap.TryGetValue(name, out IFunction value);
            return value.Apply(args);
        }

        public Interpretator()
        {
            functionMap = new Dictionary<string, IFunction>
            {
                { Functions.MIN.ToString(), new Min() },
                { Functions.POW.ToString(), new Pow() },
                { Functions.RAND.ToString(), new Rand() },
                { Functions.RANDOM.ToString(), new Rand() },
                { Functions.AVG.ToString(), new Avg() }
            };
        }
    }
}