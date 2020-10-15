using System;
using System.Collections.Generic;
using System.Linq;

namespace FGD.AST
{
    /// <summary>
    /// Represents a type of entity
    /// </summary>
    public class EntityClass : Declaration
    {
        public EntityClassType Type { get; }

        public string Name { get; }

        //TODO: can be multi-line from Source onwards, so needs to be reworked into its own statement type
        public string Description { get; }

        private readonly List<EditorProperty> _editorProperties;

        public IEnumerable<EditorProperty> EditorProperties => _editorProperties;

        private readonly List<MapProperty> _mapProperties;

        public IEnumerable<MapProperty> MapProperties => _mapProperties;

        public EntityClass(EntityClassType type, string name)
        {
            Type = type;
            Name = name;
            Description = string.Empty;
            _editorProperties = new List<EditorProperty>();
            _mapProperties = new List<MapProperty>();
        }

        public EntityClass(EntityClassType type, string name, string description,
            IEnumerable<EditorProperty> editorProperties, IEnumerable<MapProperty> mapProperties)
        {
            Type = type;
            Name = name;
            Description = description;
            _editorProperties = editorProperties.ToList();
            _mapProperties = mapProperties.ToList();
        }

        public override void Accept(FGDSyntaxVisitor visitor)
        {
            if (visitor is null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.VisitEntityClass(this);
        }

        public override string ToString()
        {
            var result = Name;

            if (!string.IsNullOrWhiteSpace(Description))
            {
                result += ':' + Description;
            }

            result += ":" + _editorProperties.Count + ':' + _mapProperties.Count;

            return result;
        }
    }
}
