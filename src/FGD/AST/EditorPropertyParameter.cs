using System;

namespace FGD.AST
{
    /// <summary>
    /// Represents an editor property parameter
    /// </summary>
    public class EditorPropertyParameter : SyntaxNode
    {
        public string Value { get; }

        public bool IsQuoted { get; }

        public EditorPropertyParameter(string value, bool isQuoted = false)
        {
            Value = value;
            IsQuoted = isQuoted;
        }

        public override void Accept(FGDSyntaxVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.VisitEditorPropertyParameter(this);
        }

        public string GetStringRepresentation()
        {
            return IsQuoted ? $"\"{Value}\"" : Value;
        }

        public override string ToString()
        {
            return GetStringRepresentation();
        }
    }
}
