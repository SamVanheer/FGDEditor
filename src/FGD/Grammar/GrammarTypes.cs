using System.Linq;

namespace FGD.Grammar
{
    public static class GrammarTypes
    {
        public static readonly FGDGrammar HalfLife1 = new FGDGrammar(
                new[]
                {
                    new DeclarationGrammar("BaseClass", DeclarationType.EntityClass),
                    new DeclarationGrammar("PointClass", DeclarationType.EntityClass),
                    new DeclarationGrammar("SolidClass", DeclarationType.EntityClass)
                },
                new[]
                {
                    new EditorPropertyGrammar("base", new[]{ new EditorPropertyParameterGrammar(DataType.EntityClassReference, isOptional: true, isVariadic: true) }),
                    new EditorPropertyGrammar("size", new[]{ new EditorPropertyParameterGrammar(DataType.Int3), new EditorPropertyParameterGrammar(DataType.Int3, isOptional: true) }),
                    new EditorPropertyGrammar("color", new[]{ new EditorPropertyParameterGrammar(DataType.Int3) }),
                    new EditorPropertyGrammar("iconsprite", new[]{ new EditorPropertyParameterGrammar(DataType.FilePath) }),
                    new EditorPropertyGrammar("studio", new[]{ new EditorPropertyParameterGrammar(DataType.FilePath, isOptional: true) }),
                    new EditorPropertyGrammar("sprite", new[]{ new EditorPropertyParameterGrammar(DataType.FilePath, isOptional: true) }),
                    new EditorPropertyGrammar("decal", Enumerable.Empty<EditorPropertyParameterGrammar>()),
                },
                new[]
                {
                    new KeyValueType("string", DataType.Text, KeyValueListType.None),
                    new KeyValueType("integer", DataType.Int1, KeyValueListType.None),
                    new KeyValueType("choices", DataType.Int1, KeyValueListType.IndexedList),
                    new KeyValueType("flags", DataType.Int1, KeyValueListType.FlagList),
                    new KeyValueType("target_source", DataType.Text, KeyValueListType.None),
                    new KeyValueType("target_destination", DataType.Text, KeyValueListType.None),
                    new KeyValueType("color255", DataType.Int3, KeyValueListType.None),
                    new KeyValueType("studio", DataType.FilePath, KeyValueListType.None),
                    new KeyValueType("sprite", DataType.FilePath, KeyValueListType.None),
                    new KeyValueType("sound", DataType.FilePath, KeyValueListType.None),
                    new KeyValueType("decal", DataType.Text, KeyValueListType.None)
                }
                );
    }
}
