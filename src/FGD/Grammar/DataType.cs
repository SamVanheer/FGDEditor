namespace FGD.Grammar
{
    public enum DataType
    {
        Text, /// <summary>Plain text</summary>
        FilePath, /// <summary>Similar to <see cref="Text"/>, but with specific checks for paths</summary>
        EntityClassReference, /// <summary>Reference to another entity class</summary>
        Int1, /// <summary>One integer value</summary>
        Int2, /// <summary>Two integer values</summary>
        Int3, /// <summary>Three integer values</summary>
        Int4, /// <summary>Four integer values</summary>
        Float1, /// <summary>One floating point value</summary>
        Float2, /// <summary>Two floating point values</summary>
        Float3, /// <summary>Three floating point values</summary>
        Float4, /// <summary>Four floating point values</summary>
    }
}
