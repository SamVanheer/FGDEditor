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

        public FGDDocument(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }
    }
}
