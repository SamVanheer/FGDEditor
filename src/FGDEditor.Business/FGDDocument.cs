using FGD.AST;

namespace FGDEditor.Business
{
    public class FGDDocument
    {
        /// <summary>
        /// The syntax tree backing this document
        /// Can be replaced to reflect changes made
        /// </summary>
        public SyntaxTree SyntaxTree { get; set; }

        /// <summary>
        /// Whether this document has changes that have not been saved to the file
        /// </summary>
        public bool HasUnsavedChanges { get; set; }

        public FGDDocument(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }
    }
}
