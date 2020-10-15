using System;

namespace FGD.Serialization
{
    public readonly struct Token : IEquatable<Token>
    {
        private readonly string? _text;

        public string Text => _text ?? string.Empty;

        public TokenType Type { get; }

        public int Line { get; }

        public int Column { get; }

        public bool IsValid => !(_text is null);

        public static Token EOF => new Token();

        public Token(string text, TokenType type, int line, int column)
        {
            _text = text;
            Type = type;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return _text is null ? "None" : $"{Type}:\"{Text}\"({Line + 1}, {Column + 1})";
        }

        public override bool Equals(object? obj)
        {
            return obj is Token token && Equals(token);
        }

        public bool Equals(Token other)
        {
            return _text == other._text &&
                   Type == other.Type &&
                   Line == other.Line &&
                   Column == other.Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_text, Type, Line, Column);
        }

        public static bool operator ==(Token left, Token right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Token left, Token right)
        {
            return !(left == right);
        }
    }
}
