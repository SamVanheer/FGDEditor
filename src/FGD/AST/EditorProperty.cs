using System;
using System.Collections.Generic;
using System.Linq;

namespace FGD.AST
{
    /// <summary>
    /// Represents an editor property
    /// </summary>
    public class EditorProperty : SyntaxNode
    {
        public string Name { get; }

        private readonly List<EditorPropertyParameter> _parameters;

        public IEnumerable<EditorPropertyParameter> Parameters => _parameters;

        public EditorProperty(string name, IEnumerable<EditorPropertyParameter> parameters)
        {
            Name = name;
            _parameters = parameters.ToList();
        }

        public override void Accept(FGDSyntaxVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.VisitEditorProperty(this);
        }

        public string GetStringRepresentation()
        {
            return $"{Name}({string.Join(", ", _parameters)})";
        }

        public override string ToString()
        {
            return GetStringRepresentation();
        }
    }
}
