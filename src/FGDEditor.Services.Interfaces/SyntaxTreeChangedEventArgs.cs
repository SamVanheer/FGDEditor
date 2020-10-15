using FGD.AST;
using System;

namespace FGDEditor.Services.Interfaces
{
    public sealed class SyntaxTreeChangedEventArgs : EventArgs
    {
        public SyntaxTree? Previous { get; }
        public SyntaxTree? Current { get; }

        public SyntaxTreeChangedEventArgs(SyntaxTree? previous, SyntaxTree? current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
