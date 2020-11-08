namespace FGD.Grammar
{
    public enum KeyValueListType
    {
        /// <summary>
        /// Keyvalue has no list
        /// </summary>
        None,

        /// <summary>
        /// List of integers with string descriptions
        /// </summary>
        IndexedList,

        /// <summary>
        /// List of power-of-2 integers with string descriptions with integer default value (0 or 1)
        /// </summary>
        FlagList,
    }
}
