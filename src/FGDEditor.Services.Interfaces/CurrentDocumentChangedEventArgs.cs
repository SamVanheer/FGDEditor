using FGDEditor.Business;
using System;

namespace FGDEditor.Services.Interfaces
{
    public sealed class CurrentDocumentChangedEventArgs : EventArgs
    {
        public FGDDocument? Previous { get; }
        public FGDDocument? Current { get; }

        public CurrentDocumentChangedEventArgs(FGDDocument? previous, FGDDocument? current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
