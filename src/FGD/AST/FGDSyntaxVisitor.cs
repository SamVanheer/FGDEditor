using System;

namespace FGD.AST
{
    /// <summary>
    /// Visits the single node passed into the Visit method
    /// </summary>
    public abstract class FGDSyntaxVisitor
    {
        public void Visit(SyntaxNode node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            node.Accept(this);
        }

        public virtual void VisitEditorProperty(EditorProperty editorProperty)
        {
        }

        public virtual void VisitEditorPropertyParameter(EditorPropertyParameter editorPropertyParameter)
        {
        }

        public virtual void VisitEntityClass(EntityClass entityClass)
        {
        }

        public virtual void VisitKeyValueMapProperty(KeyValueMapProperty keyValueMapProperty)
        {
        }

        public virtual void VisitKeyValueChoice(KeyValueChoice keyValueChoice)
        {
        }

        public virtual void VisitSingleLineComment(SingleLineComment singleLineComment)
        {
        }
    }
}
