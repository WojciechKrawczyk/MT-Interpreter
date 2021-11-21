namespace Interpreter.Tokens
{
    public enum TokenType
    {
        //key words
        Program,
        Class,
        Def,
        Int,
        Bool,
        False,
        True,
        Void,
        If,
        Else,
        While,
        Return,

        //brackets
        RoundOpenBracket,
        RoundCloseBracker,
        CurlyOpenBracket,
        CurlyCloseBracket,

        //math operators
        Plus,
        Minus,
        Multiplication,
        Division,
        Modulo,

        //relative operators
        Less,
        Grater,
        Equal,
        NotEqual,
        LessOrEqual,
        GraterOrEqual,

        //logical operators
        And,
        Or,
        Not,

        //literals
        IntLiteral,
        BoolLiteral,
        StringLiteral,

        //others
        Identifier,
        Assign,
        Semicolon,
        Comma,
        Dot,
        EndOfFile,
        Invalid
    }
}
