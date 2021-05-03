﻿namespace Interpretator
{
    public enum LexemeType
    {
        NONE,
        LEFT_BRACKET,
        RIGHT_BRACKET,
        OP_PLUS,
        OP_MINUS, 
        OP_MUL, 
        OP_DIV,
        NUMBER,
        NAME,
        COMMA,
        EOF
    }
}