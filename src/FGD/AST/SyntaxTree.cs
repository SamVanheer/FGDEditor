using System.Collections.Generic;
using System.Linq;

namespace FGD.AST
{
    /// <summary>
    /// Represents an entire FGD data structure
    /// </summary>
    public sealed class SyntaxTree
    {
        private readonly List<Declaration> _declarations;

        /// <summary>
        /// A list of all declarations, in declaration order
        /// </summary>
        public IEnumerable<Declaration> Declarations => _declarations;

        /// <summary>
        /// Creates an empty syntax tree with no declarations
        /// </summary>
        public SyntaxTree()
        {
            _declarations = new List<Declaration>();
        }

        /// <summary>
        /// Creates a syntax tree from the given list of declarations
        /// </summary>
        /// <param name="declarations"></param>
        public SyntaxTree(IEnumerable<Declaration> declarations)
        {
            _declarations = declarations.ToList();
        }
    }
}
