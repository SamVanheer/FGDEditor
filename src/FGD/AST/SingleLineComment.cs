using System;

namespace FGD.AST
{
    /// <summary>
    /// Represents a single line comment
    /// TODO: should not be a declaration
    /// </summary>
    public class SingleLineComment : Declaration
    {
        public string Text { get; set; } = string.Empty;

        public SingleLineComment(string text)
        {
            Text = text;
        }

        public override void Accept(FGDSyntaxVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.VisitSingleLineComment(this);
        }
    }
}
