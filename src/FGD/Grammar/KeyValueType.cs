namespace FGD.Grammar
{
    /// <summary>
    /// Specifies a keyvalue data type
    /// </summary>
    public sealed class KeyValueType
    {
        public string Name { get; }

        public DataType DataType { get; }

        public KeyValueListType ListType { get; }

        public KeyValueType(string name, DataType dataType, KeyValueListType listType)
        {
            Name = name;
            DataType = dataType;
            ListType = listType;
        }
    }
}
