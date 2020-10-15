namespace FGD.AST
{
    /// <summary>
    /// Base class for all syntax nodes
    /// </summary>
    public abstract class SyntaxNode
    {
        public abstract void Accept(FGDSyntaxVisitor visitor);
    }
}
