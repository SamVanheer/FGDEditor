using FGDEditor.Business;
using System;

namespace FGDEditor.Services.Interfaces
{
    public sealed class DocumentChangedEventArgs : EventArgs
    {
        public FGDDocument? Previous { get; }
        public FGDDocument? Current { get; }

        public DocumentChangedEventArgs(FGDDocument? previous, FGDDocument? current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
