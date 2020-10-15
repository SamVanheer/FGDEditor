using System;

namespace FGD.Serialization
{
    public class FGDParseException : Exception
    {
        public int Line { get; } = -1;

        public int Column { get; } = -1;

        public FGDParseException()
        {
        }

        public FGDParseException(string message)
            : base(message)
        {
        }

        public FGDParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public FGDParseException(string message, int line, int column)
            : base(message)
        {
            Line = line;
            Column = column;
        }
    }
}
