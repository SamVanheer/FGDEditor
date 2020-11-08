namespace FGD.Grammar
{
    public sealed class EditorPropertyParameterGrammar
    {
        public DataType Type { get; }

        /// <summary>
        /// Optional parameters can have zero or one (or more if variadic) values
        /// This must be the last parameter for the property
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        /// Variadic parameters are parameters that can have more than one value
        /// This must be the last parameter for the property
        /// </summary>
        public bool IsVariadic { get; }

        public EditorPropertyParameterGrammar(DataType type, bool isOptional = false, bool isVariadic = false)
        {
            Type = type;
            IsOptional = isOptional;
            IsVariadic = isVariadic;
        }
    }
}
