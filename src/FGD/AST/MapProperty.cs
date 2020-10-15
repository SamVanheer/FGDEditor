namespace FGD.AST
{
    /// <summary>
    /// Base class for map properties
    /// </summary>
    public abstract class MapProperty : SyntaxNode
    {
        public MapPropertyType PropertyType { get; }

        protected MapProperty(MapPropertyType propertyType)
        {
            PropertyType = propertyType;
        }
    }
}
