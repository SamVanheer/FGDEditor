using System.Globalization;
using System.IO;
using System.Text;

namespace FGD.Serialization
{
    /// <summary>
    /// Given a stream of text, returns individual tokens from the stream
    /// Whitespace is preserved
    /// </summary>
    public class FGDLexer
    {
        private readonly TextReader _reader;

        private int _line;

        private int _column;

        private Token? _peekedToken;

        /// <summary>
        /// If true whitespace tokens will be discarded
        /// Default false
        /// </summary>
        public bool SkipWhitespace { get; set; }

        /// <summary>
        /// If true comment tokens will be discarded
        /// Default false
        /// </summary>
        public bool SkipComments { get; set; }

        public FGDLexer(TextReader reader)
        {
            _reader = reader;
        }

        public Token GetNextToken()
        {
            if (_peekedToken.HasValue)
            {
                var peekedToken = _peekedToken.Value;
                _peekedToken = null;
                return peekedToken;
            }

            Token token;

            for (token = GetNextTokenCore();
                token.IsValid && ((SkipWhitespace && (token.Type == TokenType.Whitespace)) || (SkipComments && token.Type == TokenType.SingleLineComment));
                token = GetNextTokenCore())
            {
            }

            return token;
        }

        private Token GetNextTokenCore()
        {
            var previousColumn = _column;

            var nextCharacter = ReadNextCharacter();

            if (nextCharacter == -1)
            {
                return Token.EOF;
            }

            var character = (char)nextCharacter;

            switch (character)
            {
                case '=': return new Token("=", TokenType.Assignment, _line, previousColumn);
                case ':': return new Token(":", TokenType.Colon, _line, previousColumn);
                case ',': return new Token(",", TokenType.Comma, _line, previousColumn);

                case '(': return new Token("(", TokenType.ParenthesisOpen, _line, previousColumn);
                case ')': return new Token(")", TokenType.ParenthesisClose, _line, previousColumn);
                case '[': return new Token("[", TokenType.BracketOpen, _line, previousColumn);
                case ']': return new Token("]", TokenType.BracketClose, _line, previousColumn);

                case ' ':
                case '\t':
                    {
                        //Find the end of this whitespace
                        //Seek no further than the end of the line
                        var text = new StringBuilder();

                        text.Append(character);

                        for (var next = _reader.Peek(); next == ' ' || next == '\t'; next = _reader.Peek())
                        {
                            text.Append((char)next);
                            _reader.Read();
                        }

                        //Don't add the character that was already read
                        _column += text.Length - 1;

                        return new Token(text.ToString(), TokenType.Whitespace, _line, previousColumn);
                    }

                //Treated as zero-length whitespace
                case '\r':
                case '\n': return new Token(string.Empty, TokenType.Whitespace, _line - 1, previousColumn);

                case '/':
                    {
                        EnsureNotEOF("parsing single line comment");

                        var peeked = _reader.Peek();

                        if (peeked != '/')
                        {
                            //If it's a control character show the hex numeric value
                            var printablePeeked = char.IsControl((char)peeked)
                                ? "0X" + peeked.ToString("X", CultureInfo.InvariantCulture)
                                : ((char)peeked).ToString();

                            throw new InvalidSyntaxException($"Expected '/', got \'{printablePeeked}\'", _line, _column);
                        }

                        var line = _line;

                        //Skip the second '/'
                        _reader.Read();

                        var text = _reader.ReadLine();

                        ++_line;
                        _column = 0;

                        //It's possible for text to be null here if the end of the file is a comment followed by EOF
                        return new Token(text ?? string.Empty, TokenType.SingleLineComment, line, previousColumn);
                    }

                case '@':
                    {
                        //Find the end of this declaration
                        var text = ReadUnquotedString();

                        if (text.Length == 0)
                        {
                            throw new InvalidSyntaxException("Declaration must contain a valid identifier", _line, previousColumn + 1);
                        }

                        return new Token(text, TokenType.Declaration, _line, previousColumn);
                    }

                case '\"':
                    {
                        //Find the end of this string
                        var text = new StringBuilder();

                        for (var next = _reader.Peek(); next != '\"'; next = _reader.Peek())
                        {
                            EnsureNotEOF("parsing quoted string");

                            ReadNextCharacter();

                            text.Append((char)next);
                        }

                        //Read the terminating quote
                        _reader.Read();

                        //Account for the terminating quote
                        ++_column;

                        return new Token(text.ToString(), TokenType.QuotedString, _line, previousColumn);
                    }

                default:
                    {
                        var text = character + ReadUnquotedString();

                        return new Token(text, TokenType.UnquotedString, _line, previousColumn);
                    }
            }
        }

        public Token PeekNextToken()
        {
            if (_peekedToken.HasValue)
            {
                return _peekedToken.Value;
            }

            _peekedToken = GetNextToken();

            return _peekedToken.Value;
        }

        private void EnsureNotEOF(string currentAction)
        {
            if (_reader.Peek() == -1)
            {
                throw new InvalidSyntaxException($"Unexpected EOF while {currentAction}", _line, _column);
            }
        }

        private int ReadNextCharacter()
        {
            var character = _reader.Read();

            if (character != -1)
            {
                if (character == '\n')
                {
                    ++_line;
                    _column = 0;
                }
                else if (character == '\r')
                {
                    if (_reader.Peek() == '\n')
                    {
                        _reader.Read();
                    }

                    ++_line;
                    _column = 0;
                }
                else
                {
                    ++_column;
                }
            }

            return character;
        }

        private string ReadUnquotedString()
        {
            var text = new StringBuilder();

            for (var next = _reader.Peek();
                char.IsLetterOrDigit((char)next)
                || next == '_';
                next = _reader.Peek())
            {
                text.Append((char)next);
                _reader.Read();
            }

            _column += text.Length;

            return text.ToString();
        }
    }
}
