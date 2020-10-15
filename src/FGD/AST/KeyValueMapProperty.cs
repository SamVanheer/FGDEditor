using System;
using System.Collections.Generic;
using System.Linq;

namespace FGD.AST
{
    /// <summary>
    /// Represents a key-value map property
    /// </summary>
    public class KeyValueMapProperty : MapProperty
    {
        public string Name { get; }

        //TODO: needs something more complex that can indicate if it's a list, flags, etc
        public string Type { get; }

        public string Description { get; } = string.Empty;

        public string DefaultValue { get; } = string.Empty;

        //TODO: use a dedicated collection type
        public List<KeyValueChoice> Choices { get; } = new List<KeyValueChoice>();

        public KeyValueMapProperty(string name, string type)
            : base(MapPropertyType.KeyValue)
        {
            Name = name;
            Type = type;
        }

        public KeyValueMapProperty(string name, string type, string description, string defaultValue, IEnumerable<KeyValueChoice> choices)
            : this(name, type)
        {
            Description = description;
            DefaultValue = defaultValue;
            Choices = choices.ToList();
        }

        public override void Accept(FGDSyntaxVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.VisitKeyValueMapProperty(this);
        }

        public override string ToString()
        {
            var result = $"{Name}({Type})";

            if (!string.IsNullOrWhiteSpace(Description))
            {
                result += ':' + Description;
            }

            if (!string.IsNullOrWhiteSpace(DefaultValue))
            {
                result += ':' + DefaultValue;
            }

            if (Choices.Count > 0)
            {
                result += ":" + Choices.Count;
            }

            return result;
        }
    }
}
