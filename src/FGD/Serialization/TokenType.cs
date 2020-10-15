namespace FGD.Serialization
{
    public enum TokenType
    {
        EndOfFile = 0,
        Whitespace,
        SingleLineComment,
        Declaration,
        Assignment,
        Colon,
        Comma,
        QuotedString,
        UnquotedString,
        BracketOpen,
        BracketClose,
        ParenthesisOpen,
        ParenthesisClose,
    }
}
