using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FGD.Grammar
{
    /// <summary>
    /// Defines a grammar used by an FGD file
    /// </summary>
    public sealed class FGDGrammar
    {
        public ImmutableDictionary<string, DeclarationGrammar> Declarations { get; }

        public ImmutableDictionary<string, EditorPropertyGrammar> EditorProperties { get; }

        public ImmutableDictionary<string, KeyValueType> KeyValueTypes { get; }

        public FGDGrammar(
            IEnumerable<DeclarationGrammar> declarations,
            IEnumerable<EditorPropertyGrammar> editorProperties,
            IEnumerable<KeyValueType> keyValueTypes)
        {
            Declarations = declarations.ToImmutableDictionary(d => d.Name, StringComparer.InvariantCultureIgnoreCase);
            EditorProperties = editorProperties.ToImmutableDictionary(e => e.Name, StringComparer.InvariantCultureIgnoreCase);
            KeyValueTypes = keyValueTypes.ToImmutableDictionary(k => k.Name, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
