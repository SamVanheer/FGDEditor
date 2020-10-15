using System;

namespace FGD.AST
{
    public sealed class KeyValueChoice : SyntaxNode
    {
        public string Value { get; set; }

        public string Description { get; set; }

        public string DefaultValue { get; set; }

        public KeyValueChoice(string value, string description, string defaultValue)
        {
            Value = value;
            Description = description;
            DefaultValue = defaultValue;
        }

        public override void Accept(FGDSyntaxVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.VisitKeyValueChoice(this);
        }
    }
}
