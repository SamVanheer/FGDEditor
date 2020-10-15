using System;

namespace FGD.Serialization
{
    public sealed class InvalidSyntaxException : Exception
    {
        public int Line { get; } = -1;

        public int Column { get; } = -1;

        public InvalidSyntaxException()
        {
        }

        public InvalidSyntaxException(string message)
            : base(message)
        {
        }

        public InvalidSyntaxException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidSyntaxException(string message, int line, int column)
            : base(message)
        {
            Line = line;
            Column = column;
        }
    }
}
