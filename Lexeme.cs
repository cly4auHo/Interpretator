namespace Interpretator
{
    public class Lexeme
    {
        public LexemeType Type { get; private set; }
        public string Value { get; private set; }

        public Lexeme(LexemeType type, string value)
        {
            Type = type;
            Value = value;
        }

        public Lexeme(LexemeType type, char value)
        {
            Type = type;
            Value = value.ToString();
        }

        public override string ToString()       
            => "Lexeme{type=" + Type + ", value=" + Value + "}";      
    }
}
