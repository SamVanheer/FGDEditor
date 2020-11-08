using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FGD.Grammar
{
    /// <summary>
    /// Specifies an editor property
    /// </summary>
    public sealed class EditorPropertyGrammar
    {
        public string Name { get; }

        public ImmutableArray<EditorPropertyParameterGrammar> Parameters { get; }

        public EditorPropertyGrammar(string name, IEnumerable<EditorPropertyParameterGrammar> parameters)
        {
            Name = name;
            Parameters = parameters.ToImmutableArray();

            //Validate parameters
            var encounteredOptionalOrVariadic = false;

            foreach (var parameter in Parameters)
            {
                if (encounteredOptionalOrVariadic)
                {
                    throw new ArgumentException("Optional and/or variadic parameters must be last", nameof(parameters));
                }

                if (parameter.IsOptional || parameter.IsVariadic)
                {
                    encounteredOptionalOrVariadic = true;
                }
            }
        }
    }
}
