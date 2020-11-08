namespace FGD.Grammar
{
    public sealed class DeclarationGrammar
    {
        public string Name { get; }

        public DeclarationType Type { get; }

        public DeclarationGrammar(string name, DeclarationType type)
        {
            Name = name;
            Type = type;
        }
    }
}
